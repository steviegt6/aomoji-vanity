using JetBrains.Annotations;
using Terraria.ModLoader;

namespace AomojiVanity.API.Loading;

/// <summary>
///     Gets loaded when the mod is constructed, instead of during the regular
///     load cycle.
/// </summary>
/// <remarks>
///     Useful for loading things that need to be loaded before the mod is
///     loaded.
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithInheritors)]
internal interface IConstructedLoadable {
    /// <summary>
    ///     Called when the mod is constructed.
    /// </summary>
    /// <param name="mod">The mod that is being constructed.</param>
    void Load(Mod mod);

    /// <summary>
    ///     Called when the mod is unloaded.
    /// </summary>
    void Unload();
}
