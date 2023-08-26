using System;

namespace AomojiCommonLibs.Reflection.RuntimeAccessor;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MethodAccessorAttribute : Attribute {
    public string Name { get; }

    public MethodAccessorAttribute(string name) {
        Name = name;
    }
}
