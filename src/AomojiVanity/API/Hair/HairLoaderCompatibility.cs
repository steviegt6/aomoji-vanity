using System;
using System.Collections.Generic;
using AomojiVanity.API.Hijacking;
using AomojiVanity.API.ModCall;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hair;

/// <summary>
///     Hijacks the HairLoader mod for compatibility with our API.
/// </summary>
internal sealed class HairLoaderCompatibility : ModHijack {
    public override IEnumerable<Mod> HijackTargets {
        get {
            if (ModLoader.GetMod("HairLoader") is { } hairLoader)
                yield return hairLoader;
        }
    }

    public override HijackResult HijackCall(Mod mod, params object?[]? args) {
        if (mod.Name != "HairLoader")
            return HijackResult.NOT_HIJACKED;

        Mod.Logger.Debug($"HairLoaderCompatibility: '{string.Join(", ", args ?? System.Array.Empty<object?>())}'.");
        if (args is null || args.Length == 0)
            return HijackResult.NOT_HIJACKED;

        switch (args[0]) {
            case "RegisterHairStyle":
                if (args.Length != 13) {
                    Mod.Logger.Debug($"HairLoaderCompatibility: Failed to validate Mod.Call arguments, not hijacked.");
                    return HijackResult.NOT_HIJACKED;
                }

                break;

            case "HairLoaderUnlockCondition":
                if (args.Length != 2) {
                    Mod.Logger.Debug($"HairLoaderCompatibility: Failed to validate Mod.Call arguments, not hijacked.");
                    return HijackResult.NOT_HIJACKED;
                }

                break;

            case "ChangePlayerHairStyle":
                if (!ModCallTypeValidator.Validate(args, out string _, out string? modClassName, out string? hairEntryName, out object? playerId)) {
                    Mod.Logger.Debug($"HairLoaderCompatibility: Failed to validate Mod.Call arguments, not hijacked.");
                    return HijackResult.NOT_HIJACKED;
                }
                
                ChangePlayerHairStyle(modClassName, hairEntryName, Convert.ToInt32(playerId));
                return new HijackResult(true, "Success");

            default:
                Mod.Logger.Debug($"HairLoaderCompatibility: Unknown command '{args[0]}', not hijacked.");
                return HijackResult.NOT_HIJACKED;
        }
    }

    private void ChangePlayerHairStyle(string modClassName, string hairEntryName, int playerId) { }
}
