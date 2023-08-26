using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

[AttributeUsage(AttributeTargets.Property)]
public sealed class MethodAccessorAttribute : Attribute {
    public string Name { get; }

    public MethodAccessorAttribute(string name) {
        Name = name;
    }
}
