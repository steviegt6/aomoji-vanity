using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

/// <summary>
///     Indicates that a method to be implemented by the
///     <see cref="RuntimeAccessorGenerator"/> should invoke the method of
///     <see cref="Name"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MethodAccessorAttribute : Attribute {
    public string Name { get; }
    
    public bool IsStatic { get; set; } = false;

    public MethodAccessorAttribute(string name) {
        Name = name;
    }
}
