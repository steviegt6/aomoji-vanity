using System;
using System.Collections.Generic;
using System.Reflection;
using AomojiCommonLibs.Reflection.RuntimeAccessor;
using JetBrains.Annotations;
using Lucifer.Content.Lighting;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.Localization;
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

        IL_IngameOptions.Draw += AddLightingEngineOptionsToIngameOptions;
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

    private void AddLightingEngineOptionsToIngameOptions(ILContext il) {
        var buttonNumIndex = -1;
        var sbIndex = -1;
        // ILLabel? labelForAfterButtonWeWantToInsertAfter = null;

        var c = new ILCursor(il);
        c.GotoNext(x => x.MatchLdstr("UI.LightMode_"));
        c.GotoPrev(x => x.MatchLdloc(out buttonNumIndex));
        c.GotoNext(x => x.MatchLdarg(out sbIndex));
        c.GotoNext(MoveType.Before, x => x.MatchLdloc(buttonNumIndex));
        var startIndex = c.Index;
        c.GotoNext(MoveType.After, x => x.MatchCall(out _));
        var endIndex = c.Index;
        var drawRightSideInstructions = il.Instrs.ToArray()[startIndex..endIndex];
        c.GotoNext(MoveType.Before, x => x.MatchLdloc(buttonNumIndex));
        startIndex = c.Index;
        c.GotoNext(MoveType.After, x => x.MatchLdloc(out _));
        endIndex = c.Index;
        var conditionalInstructions = il.Instrs.ToArray()[startIndex..endIndex];
        /*c.GotoNext(x => x.MatchBrfalse(out labelForAfterButtonWeWantToInsertAfter));
        if (labelForAfterButtonWeWantToInsertAfter is null)
            throw new InvalidOperationException("Could not find label for after button we want to insert after.");

        c.Goto(labelForAfterButtonWeWantToInsertAfter.Target, MoveType.After);
        c.Index--;*/
        c.GotoNext(x => x.MatchLdloc(buttonNumIndex));
        c.GotoNext(MoveType.After, x => x.MatchStloc(buttonNumIndex));

        var label = c.DefineLabel();

        c.Emit(OpCodes.Ldarg, sbIndex);
        c.EmitDelegate(() => Language.GetTextValue("Mods.Lucifer.UI.LightingEngine", CurrentLightingEngine.DisplayName.Value));
        foreach (var instruction in drawRightSideInstructions)
            c.Emit(instruction.OpCode, instruction.Operand);
        c.Emit(OpCodes.Brfalse, label);
        foreach (var instruction in conditionalInstructions)
            c.Emit(instruction.OpCode, instruction.Operand);
        c.Emit(OpCodes.Brfalse, label);
        c.EmitDelegate(CycleProvider);
        c.MarkLabel(label);
        
        // buttonNum++;
        c.Emit(OpCodes.Ldloc, buttonNumIndex);
        c.Emit(OpCodes.Ldc_I4_1);
        c.Emit(OpCodes.Add);
        c.Emit(OpCodes.Stloc, buttonNumIndex);
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
