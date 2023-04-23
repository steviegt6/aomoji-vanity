using System.Collections.Generic;
using AomojiVanity.API.Hijacking;
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

        Mod.Logger.Debug("HairLoader compatibility hijack invoked.");
        return new HijackResult(true, null);
    }
}
