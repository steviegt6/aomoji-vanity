using System;
using System.Collections.Generic;
using System.Reflection;
using AomojiCommonLibs.Reflection.RuntimeAccessor;
using JetBrains.Annotations;
using Lucifer.Content.Lighting;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Lucifer.API;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class LightingEngineLoader : ModSystem {
    private interface ILightingAccessor {
        [FieldAccessor("_activeEngine", IsStatic = true)]
        public ILightingEngine ActiveEngine { get; set; }

        [FieldAccessor("_mode", IsStatic = true)]
        public LightMode Mode { get; set; }
    }

    private LightingEngineProvider currentLightingEngine = null!;

    public LightingEngineProvider CurrentLightingEngine {
        get => currentLightingEngine;

        set {
            currentLightingEngine = value;
            var mode = lightingAccessor.Mode;
            lightingAccessor.ActiveEngine = GetLightingEngine(currentLightingEngine, ref mode);
            lightingAccessor.Mode = mode;
        }
    }

    private readonly ILightingAccessor lightingAccessor = RuntimeAccessorGenerator.GenerateAccessor<Lighting, ILightingAccessor>(null);
    private readonly List<LightingEngineProvider> engineProviders = new();

    private Hook? setModeHook;

    public void Register(LightingEngineProvider provider) {
        engineProviders.Add(provider);
    }

    public override void Load() {
        base.Load();

        setModeHook = new Hook(
            typeof(Lighting).GetMethod("set_Mode", BindingFlags.Public | BindingFlags.Static)!,
            typeof(LightingEngineLoader).GetMethod(nameof(SetMode), BindingFlags.NonPublic | BindingFlags.Instance)!,
            this
        );
    }

    public override void OnModLoad() {
        base.OnModLoad();

        Mod.AddContent(CurrentLightingEngine = new VanillaLightingEngineProvider());
    }

    public override void Unload() {
        base.Unload();

        setModeHook?.Dispose();
        setModeHook = null;
    }

    internal void CycleProvider() {
        var index = engineProviders.IndexOf(CurrentLightingEngine);
        CurrentLightingEngine = engineProviders[(index + 1) % engineProviders.Count];
    }

    private static ILightingEngine GetLightingEngine(LightingEngineProvider provider, ref LightMode mode) {
        var recursionDepth = 0;

        while (true) {
            var engine = provider.GetLightingEngine(mode);

            if (engine is not null)
                return engine;

            if (recursionDepth >= 3)
                throw new InvalidOperationException("No lighting engine found.");

            mode = mode switch {
                LightMode.White => LightMode.Retro,
                LightMode.Retro => LightMode.Trippy,
                LightMode.Trippy => LightMode.Color,
                LightMode.Color => LightMode.White,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };

            recursionDepth++;
        }
    }

    private void SetMode(LightMode mode) {
        // In vanilla, the mode gets cycled up to 4 and is then corrected. So we
        // should just let it happen.
        if ((int)mode == 4) {
            lightingAccessor.Mode = mode;
            return;
        }

        var engine = GetLightingEngine(CurrentLightingEngine, ref mode);

        lightingAccessor.ActiveEngine = engine;
        lightingAccessor.Mode = mode;
        Main.renderCount = 0;
        Main.renderNow = false;
    }
}
