using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Lucifer.API;

/// <summary>
///     A lighting engine provider which may provide an
///     <see cref="ILightingEngine"/> for a given <see cref="LightMode"/>.
/// </summary>
public abstract class LightingEngineProvider : ModType {
#region ModType Impl
    protected sealed override void Register() {
        ModContent.GetInstance<LightingEngineLoader>().Register(this);
    }
#endregion

    /// <summary>
    ///     Gets the <see cref="ILightingEngine"/> for the given
    ///     <see cref="LightMode"/>.
    /// </summary>
    /// <param name="mode">The lighting mode.</param>
    /// <returns>
    ///     An <see cref="ILightingEngine"/> instance, or <see langword="null"/>
    ///     if the mode is unsupported.
    /// </returns>
    public abstract ILightingEngine? GetLightingEngine(LightMode mode);
}
