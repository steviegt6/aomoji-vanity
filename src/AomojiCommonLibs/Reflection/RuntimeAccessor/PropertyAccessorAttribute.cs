using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAccessorAttribute : Attribute {
    public string Name { get; }

    public PropertyAccessorAttribute(string name) {
        Name = name;
    }
}
