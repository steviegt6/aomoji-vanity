using JetBrains.Annotations;
using Lucifer.API;
using Terraria.Graphics.Light;

namespace Hikari.Content.Lighting;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class HikariLightingEngineProvider : LightingEngineProvider {
    private readonly ILightingEngine engine = new HikariLightingEngine();

    public override ILightingEngine? GetLightingEngine(LightMode mode) {
        return mode == LightMode.Color ? engine : null;
    }
}
