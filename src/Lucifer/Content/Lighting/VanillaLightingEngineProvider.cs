using System;
using AomojiCommonLibs.Reflection.RuntimeAccessor;
using JetBrains.Annotations;
using Lucifer.API;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Lucifer.Content.Lighting;

[Autoload(false)]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class VanillaLightingEngineProvider : LightingEngineProvider {
    private interface ILightingAccessor {
        [FieldAccessor("NewEngine", IsStatic = true)]
        LightingEngine NewEngine { get; }
    }

    private readonly ILightingAccessor lightingAccessor = RuntimeAccessorGenerator.GenerateAccessor<Terraria.Lighting, ILightingAccessor>(null);

    public override ILightingEngine? GetLightingEngine(LightMode mode) {
        switch (mode) {
            case LightMode.White:
                Terraria.Lighting.LegacyEngine.Mode = 1;
                return Terraria.Lighting.LegacyEngine;

            case LightMode.Retro:
                Terraria.Lighting.LegacyEngine.Mode = 2;
                return Terraria.Lighting.LegacyEngine;

            case LightMode.Trippy:
                Terraria.Lighting.LegacyEngine.Mode = 3;
                return Terraria.Lighting.LegacyEngine;

            case LightMode.Color:
                Terraria.Lighting.LegacyEngine.Mode = 0;
                Terraria.Lighting.OffScreenTiles = 35;
                return lightingAccessor.NewEngine;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}
