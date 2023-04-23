using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hijacking;

/// <summary>
///     Loads <see cref="ModHijack"/> instances.
/// </summary>
internal static class ModHijackLoader {
    private static Dictionary<Mod, ModHijack> hijackers = new();
    private static Dictionary<Mod, Hook> detours = new();

    internal static void Unload() {
        foreach (var detour in detours.Values)
            detour.Dispose();

        hijackers = null!;
        detours = null!;
    }

    public static void Register(ModHijack hijack) {
        RegisterHijacker(hijack);
    }

    private static void RegisterHijacker(ModHijack hijack) {
        foreach (var mod in hijack.HijackTargets) {
            if (hijackers.ContainsKey(mod))
                throw new InvalidOperationException($"Mod '{mod.Name}' has already been hijacked by {hijackers[mod].GetType().FullName}.");

            hijackers.Add(mod, hijack);
            DetourModCallForMod(mod);
        }
    }

    private static void DetourModCallForMod(Mod mod) {
        var call = typeof(Mod).GetMethod("Call", BindingFlags.Public | BindingFlags.Instance)!;
        var hook = new Hook(call, ModCallDetour);
        hook.Apply();
        detours.Add(mod, hook);
    }

    private static object? ModCallDetour(Func<Mod, object?[]?, object?> orig, Mod mod, object?[]? args) {
        if (!hijackers.TryGetValue(mod, out var hijacker))
            return orig(mod, args);

        var result = hijacker.HijackCall(mod, args);
        return result.Hijacked ? result.Result : orig(mod, args);
    }
}
