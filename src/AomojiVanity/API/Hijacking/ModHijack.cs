using System.Collections.Generic;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hijacking;

/// <summary>
///     Redirects <see cref="Mod.Call"/> invocations of external mods.
/// </summary>
internal abstract class ModHijack : ModType {
#region Impl
    protected sealed override void Register() {
        ModHijackLoader.Register(this);
        ModTypeLookup<ModHijack>.Register(this);
    }
#endregion

    /// <summary>
    ///     Gets the mods that this hijacker will hijack.
    /// </summary>
    public abstract IEnumerable<Mod> HijackTargets { get; }

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
    public abstract HijackResult HijackCall(Mod mod, params object?[]? args);
}
