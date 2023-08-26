using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FieldAccessorAttribute : Attribute {
    public string Name { get; }

    public FieldAccessorAttribute(string name) {
        Name = name;
    }
}
