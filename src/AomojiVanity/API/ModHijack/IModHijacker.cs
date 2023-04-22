using System.Collections.Generic;
using Terraria.ModLoader;

namespace AomojiVanity.API.ModHijack;

/// <summary>
///     Provides an interface for redirecting <see cref="Mod.Call"/> invocations
///     of external mods.
/// </summary>
internal interface IModHijacker {
    /// <summary>
    ///     Gets the mods that this hijacker will hijack.
    /// </summary>
    IEnumerable<string> HijackTargets { get; }

    /// <summary>
    ///     Hijacks a call to <see cref="Mod.Call"/>.
    /// </summary>
    /// <param name="mod">The mod that is being called.</param>
    /// <param name="args">
    ///     The arguments that were passed to <see cref="Mod.Call"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="HijackResult"/> that indicates whether the call was
    ///     hijacked and the result of the hijack.
    /// </returns>
    HijackResult HijackCall(Mod mod, params object?[]? args);
}
