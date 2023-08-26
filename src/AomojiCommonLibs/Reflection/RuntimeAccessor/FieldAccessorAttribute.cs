using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

/// <summary>
///     Indicates that a property to be implemented by the
///     <see cref="RuntimeAccessorGenerator"/> should implement getters and
///     setters for the field of <see cref="Name"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FieldAccessorAttribute : Attribute {
    public string Name { get; }

    public FieldAccessorAttribute(string name) {
        Name = name;
    }
}
