using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

/// <summary>
///     Handles dynamically implementing interfaces for accessing
///     visibility-restricted members at runtime.
/// </summary>
public static class RuntimeAccessorGenerator {
    // OPTIMIZATION: Type handles have better hashcode performance.
    private static readonly ConcurrentDictionary<nint, ConcurrentDictionary<nint, Func<object?, object>>> accessor_generators = new();

    /// <summary>
    ///     Generates an accessor implementing <typeparamref name="TInterface"/>
    ///     for the given <paramref name="from"/> object of type
    ///     <typeparamref name="TSource"/>.
    /// </summary>
    /// <param name="from">The instance to access.</param>
    /// <typeparam name="TSource">The type to access members from.</typeparam>
    /// <typeparam name="TInterface">The accessor to implement.</typeparam>
    /// <returns>
    ///     An implemented instance of <typeparamref name="TInterface"/>.
    /// </returns>
    public static TInterface GenerateAccessor<TSource, TInterface>(TSource? from) {
        var func = BuildOrGetAccessorGenerator<TSource, TInterface>();
        return (TInterface)func(from);
    }

    private static Func<object?, object> BuildOrGetAccessorGenerator<TSource, TInterface>() {
        nint fromHandle = typeof(TSource).TypeHandle.Value;
        nint toHandle = typeof(TInterface).TypeHandle.Value;

        if (accessor_generators.TryGetValue(fromHandle, out var toGenerators)) {
            if (toGenerators.TryGetValue(toHandle, out var generator))
                return tFrom => generator(tFrom);

            return toGenerators[toHandle] = BuildAccessorGenerator<TSource, TInterface>();
        }

        var func = BuildAccessorGenerator<TSource, TInterface>();
        accessor_generators[fromHandle] = new ConcurrentDictionary<nint, Func<object?, object>> {
            [toHandle] = tFrom => func((TSource?)tFrom),
        };
        return func;
    }

    private static Func<object?, object> BuildAccessorGenerator<TSource, TInterface>() {
        if (!typeof(TInterface).IsInterface)
            throw new InvalidOperationException("Cannot generate accessor mapped to non-interface type.");

        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AomojiCommonLibs.RuntimeAccessorGenerators"), AssemblyBuilderAccess.RunAndCollect);
        assembly.SetCustomAttribute(new CustomAttributeBuilder(typeof(IgnoresAccessChecksToAttribute).GetConstructor(new[] { typeof(string) })!, new object[] { typeof(TSource).Assembly.GetName().Name! }));
        assembly.SetCustomAttribute(new CustomAttributeBuilder(typeof(IgnoresAccessChecksToAttribute).GetConstructor(new[] { typeof(string) })!, new object[] { typeof(TInterface).Assembly.GetName().Name! }));
        var module = assembly.DefineDynamicModule("AomojiCommonLibs.RuntimeAccessorGenerators");

        var typeName = $"{typeof(TSource).FullName}_RuntimeAccessor_{typeof(TInterface).FullName}";
        var typeBuilder = module.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, typeof(object));
        typeBuilder.AddInterfaceImplementation(typeof(TInterface));

        var instanceField = typeBuilder.DefineField("instance", typeof(TSource), FieldAttributes.Private | FieldAttributes.InitOnly);

        var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(TSource) });
        var constructorIL = constructorBuilder.GetILGenerator();
        constructorIL.Emit(OpCodes.Ldarg_0);
        constructorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
        constructorIL.Emit(OpCodes.Ldarg_0);
        constructorIL.Emit(OpCodes.Ldarg_1);
        constructorIL.Emit(OpCodes.Stfld, instanceField);
        constructorIL.Emit(OpCodes.Ret);

        foreach (var property in typeof(TInterface).GetProperties()) {
            if (property.GetMethod?.IsStatic == true || property.SetMethod?.IsStatic == true)
                continue;

            var propertyDef = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);

            if (property.GetCustomAttribute<FieldAccessorAttribute>() is { } fieldAccessor) {
                var field = typeof(TSource).GetField(fieldAccessor.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;

                var getter = typeBuilder.DefineMethod($"get_{property.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.PropertyType, Type.EmptyTypes);
                var getterIL = getter.GetILGenerator();

                if (fieldAccessor.IsStatic) {
                    getterIL.Emit(OpCodes.Ldsfld, field);
                }
                else {
                    getterIL.Emit(OpCodes.Ldarg_0);
                    getterIL.Emit(OpCodes.Ldfld, instanceField);
                    getterIL.Emit(OpCodes.Ldfld, field);
                }

                getterIL.Emit(OpCodes.Ret);
                propertyDef.SetGetMethod(getter);

                var setter = typeBuilder.DefineMethod($"set_{property.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { property.PropertyType });
                var setterIL = setter.GetILGenerator();

                if (fieldAccessor.IsStatic) {
                    setterIL.Emit(OpCodes.Ldarg_1);
                    setterIL.Emit(OpCodes.Stsfld, field);
                }
                else {
                    setterIL.Emit(OpCodes.Ldarg_0);
                    setterIL.Emit(OpCodes.Ldfld, instanceField);
                    setterIL.Emit(OpCodes.Ldarg_1);
                    setterIL.Emit(OpCodes.Stfld, field);
                }

                setterIL.Emit(OpCodes.Ret);
                propertyDef.SetSetMethod(setter);
            }
            else if (property.GetCustomAttribute<PropertyAccessorAttribute>() is { } propertyAccessor) {
                var propertyInfo = typeof(TSource).GetProperty(propertyAccessor.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;

                var getter = typeBuilder.DefineMethod($"get_{property.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, property.PropertyType, Type.EmptyTypes);
                var getterIL = getter.GetILGenerator();

                if (propertyAccessor.IsStatic) {
                    getterIL.Emit(OpCodes.Call, propertyInfo.GetMethod!);
                }
                else {
                    getterIL.Emit(OpCodes.Ldarg_0);
                    getterIL.Emit(OpCodes.Ldfld, instanceField);
                    getterIL.Emit(OpCodes.Callvirt, propertyInfo.GetMethod!);
                }

                getterIL.Emit(OpCodes.Ret);
                propertyDef.SetGetMethod(getter);

                var setter = typeBuilder.DefineMethod($"set_{property.Name}", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { property.PropertyType });
                var setterIL = setter.GetILGenerator();

                if (propertyAccessor.IsStatic) {
                    setterIL.Emit(OpCodes.Ldarg_1);
                    setterIL.Emit(OpCodes.Call, propertyInfo.SetMethod!);
                }
                else {
                    setterIL.Emit(OpCodes.Ldarg_0);
                    setterIL.Emit(OpCodes.Ldfld, instanceField);
                    setterIL.Emit(OpCodes.Ldarg_1);
                    setterIL.Emit(OpCodes.Callvirt, propertyInfo.SetMethod!);
                }

                setterIL.Emit(OpCodes.Ret);
                propertyDef.SetSetMethod(setter);
            }
        }

        foreach (var method in typeof(TInterface).GetMethods()) {
            if (method.IsStatic)
                continue;

            if (method.GetCustomAttribute<MethodAccessorAttribute>() is not { } methodAccessor)
                continue;

            var methodDef = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());

            var methodIL = methodDef.GetILGenerator();

            if (!methodAccessor.IsStatic) {
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, instanceField);
            }

            for (var i = 0; i < method.GetParameters().Length; i++)
                methodIL.Emit(OpCodes.Ldarg, i + 1);
            
            methodIL.Emit(methodAccessor.IsStatic ? OpCodes.Call : OpCodes.Callvirt, typeof(TSource).GetMethod(methodAccessor.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, method.GetParameters().Select(p => p.ParameterType).ToArray(), null)!);
            
            methodIL.Emit(OpCodes.Ret);
        }

        var type = typeBuilder.CreateType()!;
        var constructor = type.GetConstructor(new[] { typeof(TSource) })!;
        return instance => constructor.Invoke(new[] { instance });
    }
}
