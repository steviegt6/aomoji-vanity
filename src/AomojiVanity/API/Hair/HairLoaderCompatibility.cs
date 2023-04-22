using System.Collections.Generic;
using AomojiVanity.API.ModHijack;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hair; 

/// <summary>
///     Hijacks the HairLoader mod for compatibility with our API.
/// </summary>
internal sealed class HairLoaderCompatibility : IModHijacker {
    IEnumerable<string> IModHijacker.HijackTargets {
        get {
            yield return "HairLoader";
        }
    }

    HijackResult IModHijacker.HijackCall(Mod mod, params object?[]? args) {
        if (mod.Name != "HairLoader")
            return HijackResult.NOT_HIJACKED;

        return new HijackResult(true, null);
    }
}
