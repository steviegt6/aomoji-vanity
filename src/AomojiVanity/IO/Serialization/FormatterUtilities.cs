using System;
using System.Runtime.Serialization;

namespace AomojiVanity.IO.Serialization;

/// <summary>
///     Provides utilities in line with functionality provided by
///     <see cref="FormatterServices"/>.
/// </summary>
public static class FormatterUtilities {
    /// <summary>
    ///     Creates a new instance of the specified object type.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <returns>A zeroed object of the specified type.</returns>
    public static T GetUninitializedObject<T>() where T : class {
        return (T) FormatterServices.GetUninitializedObject(typeof(T));
    }

    /// <summary>
    ///     Initializes an object of the specified type. <br />
    ///     Assumes the type has a parameterless constructor.
    /// </summary>
    /// <param name="obj">The object to initialize.</param>
    /// <typeparam name="T">The type of object to initialize.</typeparam>
    public static void InitializeObject<T>(this T obj) where T : class {
        typeof(T).GetConstructor(Type.EmptyTypes)!.Invoke(obj, null);
    }
}
