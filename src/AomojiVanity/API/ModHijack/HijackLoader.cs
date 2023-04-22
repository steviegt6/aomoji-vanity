using System;
using System.Collections.Generic;
using System.Reflection;
using AomojiVanity.API.Loading;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace AomojiVanity.API.ModHijack;

/// <summary>
///     Loads <see cref="IModHijacker"/> instances.
/// </summary>
internal sealed class HijackLoader : IConstructedLoadable {
    private Dictionary<string, IModHijacker> hijackers = new();
    private Dictionary<Mod, Hook> detours = new();

    void IConstructedLoadable.Load(Mod mod) {
        LoadHijacks();
    }

    void IConstructedLoadable.Unload() {
        foreach (var detour in detours.Values)
            detour.Dispose();

        hijackers = null!;
        detours = null!;
    }

    public void RegisterHijacker(IModHijacker hijacker) {
        foreach (var mod in hijacker.HijackTargets) {
            if (hijackers.ContainsKey(mod))
                throw new InvalidOperationException($"Mod {mod} has already been hijacked by {hijackers[mod].GetType().FullName}.");

            hijackers.Add(mod, hijacker);
            DetourModCallForMod(mod);
        }
    }

    private void LoadHijacks() {
        foreach (var mod in ModLoader.Mods) {
            foreach (var type in mod.Code.GetTypes()) {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (!typeof(IModHijacker).IsAssignableFrom(type))
                    continue;

                var hijacker = (IModHijacker) Activator.CreateInstance(type)!;
                RegisterHijacker(hijacker);
            }
        }
    }

    private void DetourModCallForMod(Mod mod) {
        var call = typeof(Mod).GetMethod("Call", BindingFlags.Public | BindingFlags.Instance)!;
        var hook = new Hook(call, ModCallDetour);
        hook.Apply();
        detours.Add(mod, hook);
    }

    private object? ModCallDetour(Func<Mod, object?[]?, object?> orig, Mod mod, object?[]? args) {
        if (!hijackers.TryGetValue(mod, out var hijacker))
            return orig(mod, args);

        var result = hijacker.HijackCall(mod, args);
        return result.Hijacked ? result.Result : orig(mod, args);
    }
}
