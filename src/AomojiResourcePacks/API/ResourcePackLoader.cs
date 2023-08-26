using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Content.Sources;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AomojiResourcePacks.API;

public static class ResourcePackLoader {
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    private sealed class LoaderSystem : ModSystem {
        public LoaderSystem() {
            ResourcePackLoader.Load();
        }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public override void PostSetupContent() {
            base.PostSetupContent();

            ResourcePackLoader.PostSetupContent();
        }

        public override void OnModUnload() {
            base.OnModUnload();

            ResourcePackLoader.Unload();
        }
    }

    private static string ResourcePacksPath => Path.Combine(Main.SavePath, "aomoji", "resourcePacks.json");

    private static Dictionary<Mod, AssetSourceController>? modSourceControllers;
    private static List<ModResourcePack> modResourcePacks = new();
    private static List<string> enabledResourcePacks = new();
    private static bool resourcePacksSetThisSession;
    private static bool actingUponModSourceController;

    public static void Register(ModResourcePack resourcePack) {
        modResourcePacks.Add(resourcePack);

        if (!resourcePack.ForceEnabled)
            resourcePack.Entity.IsEnabled = enabledResourcePacks.Contains(resourcePack.Name) || resourcePack.Entity.IsEnabled;
    }

    internal static void Load() {
        Directory.CreateDirectory(Path.GetDirectoryName(ResourcePacksPath)!);

        modResourcePacks = new List<ModResourcePack>();

        IL_ResourcePack.ctor += SkipPathSettingForModdedPacks;
        On_ResourcePack.HasFile += ResourcePackHasModdedFile;
        On_ResourcePack.OpenStream += ResourcePackOpenModdedStream;
        On_ResourcePack.GetContentSource += ResourcePackGetModdedContentSource;
        On_ResourcePack.CreateIcon += ResourcePackGetRawImgIcon;

        On_ResourcePackList.FromJson += FromJsonAddModdedPacks;

        On_UIResourcePackSelectionMenu.CreatePackToggleButton += OverrideOnLeftClickWhenForceEnabled;
        On_UIResourcePackSelectionMenu.DisablePackUpdate += ShowForceEnabledTooltip;

        On_UIResourcePack.DrawSelf += DrawModdedIcon;

        On_AssetSourceController.UseResourcePacks += UpdateInUseResourcePacks;
    }

    internal static void PostSetupContent() {
        enabledResourcePacks = File.Exists(ResourcePacksPath) ? JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(ResourcePacksPath)) : GetPackStatesFromSourceController();

        // Force the current resource packs to update.
        Main.QueueMainThreadAction(() => {
            Main.AssetSourceController.UseResourcePacks(AssetInitializer.CreateResourcePackList(Main.instance.Services));
        });
    }

    internal static void Unload() {
        modResourcePacks = null!;
        enabledResourcePacks = null!;
    }

    private static List<string> GetPackStatesFromSourceController() {
        return Main.AssetSourceController.ActiveResourcePackList.EnabledPacks.OrderBy(x => x.SortingOrder).Select(ConvertPackToKey).ToList();
    }

    private static string ConvertPackToKey(ResourcePack pack) {
        if (pack is ContentSourceResourcePack { ModResourcePack: { } } contentPack)
            return contentPack.ModResourcePack.FullName;

        return pack.FullPath;
    }

    private static void SkipPathSettingForModdedPacks(ILContext il) {
        var c = new ILCursor(il);
        var label = c.DefineLabel();

        c.GotoNext(MoveType.After, x => x.MatchThrow());
        c.MarkLabel(label);

        c.Index = 0;
        c.GotoNext(MoveType.After, x => x.MatchStfld<ResourcePack>("_needsReload"));
        c.Emit(OpCodes.Ldarg_2);
        c.Emit(OpCodes.Brfalse, label);
    }

    private static bool ResourcePackHasModdedFile(On_ResourcePack.orig_HasFile orig, ResourcePack self, string fileName) {
        if (self is not ContentSourceResourcePack extended)
            return orig(self, fileName);

        return extended.RootSource?.HasAsset(fileName) ?? orig(self, fileName);
    }

    private static Stream ResourcePackOpenModdedStream(On_ResourcePack.orig_OpenStream orig, ResourcePack self, string fileName) {
        if (self is not ContentSourceResourcePack extended)
            return orig(self, fileName);

        // Modded resource packs rely on a provided content source.
        return extended.RootSource?.OpenStream(fileName) ?? orig(self, fileName);
    }

    private static IContentSource ResourcePackGetModdedContentSource(On_ResourcePack.orig_GetContentSource orig, ResourcePack self) {
        if (self is not ContentSourceResourcePack extended)
            return orig(self);

        // Modded resource packs rely on a provided content source.
        return extended.ContentSource ?? orig(self);
    }

    private static Texture2D ResourcePackGetRawImgIcon(On_ResourcePack.orig_CreateIcon orig, ResourcePack self) {
        if (self is not ContentSourceResourcePack extended)
            return orig(self);

        // Modded resource packs rely on a provided content source.
        if (extended.RootSource == null)
            return orig(self);

        // Fall back to the orig call since that handles a case where there is
        // no icon.
        if (!extended.RootSource.HasAsset("icon.rawimg"))
            return orig(self);

        using var stream = extended.RootSource.OpenStream("icon.rawimg");
        return Main.instance.Services.Get<IAssetRepository>().CreateUntracked<Texture2D>(stream, ".rawimg").Value;
    }

    private static ResourcePackList FromJsonAddModdedPacks(On_ResourcePackList.orig_FromJson orig, JArray serializedState, IServiceProvider services, string searchPath) {
        resourcePacksSetThisSession = true;

        var resourcePacks = orig(serializedState, services, searchPath).AllPacks.Concat(modResourcePacks.Select(x => x.Entity)).ToList();
        var canonicalPacks = new List<ResourcePack>();

        foreach (var pack in resourcePacks) {
            if (enabledResourcePacks.Contains(ConvertPackToKey(pack))) {
                pack.IsEnabled = true;
                pack.SortingOrder = enabledResourcePacks.IndexOf(ConvertPackToKey(pack));
            }
            else {
                pack.IsEnabled = pack is ContentSourceResourcePack { ModResourcePack.ForceEnabled: true };
            }

            canonicalPacks.Add(pack);
        }

        return new ResourcePackList(canonicalPacks);
    }

    private static UIElement OverrideOnLeftClickWhenForceEnabled(On_UIResourcePackSelectionMenu.orig_CreatePackToggleButton orig, UIResourcePackSelectionMenu self, ResourcePack resourcePack) {
        if (resourcePack is not ContentSourceResourcePack extended)
            return orig(self, resourcePack);

        // ForceEnabled is determined by the source ModResourcePack.
        if (extended.ModResourcePack is null || !extended.ModResourcePack.ForceEnabled)
            return orig(self, resourcePack);

        var button = new GroupOptionButton<bool>(true, null, null, Color.White, null, 0.8f) {
            Left = StyleDimension.FromPercent(0.5f),
            Width = StyleDimension.FromPercent(0.5f),
            Height = StyleDimension.Fill,
        };
        button.SetColorsBasedOnSelectionState(Color.Gray, Color.Gray, 0.7f, 0.7f);
        button.SetCurrentOption(true);
        button.ShowHighlightWhenSelected = false;
        button.SetPadding(0f);
        var asset = Main.Assets.Request<Texture2D>("Images/UI/TexturePackButtons");
        var image = new UIImageFramed(asset, asset.Frame(2, 2, 0, 1)) {
            HAlign = 0.5f,
            VAlign = 0.5f,
            IgnoresMouseInteraction = true,
        };
        button.Append(image);

        button.OnMouseOver += (_, _) => {
            SoundEngine.PlaySound(SoundID.MenuTick);
        };

        return button;
    }

    private static void ShowForceEnabledTooltip(On_UIResourcePackSelectionMenu.orig_DisablePackUpdate orig, UIResourcePackSelectionMenu self, UIElement affectedElement) {
        if (affectedElement is not GroupOptionButton<bool> { OptionValue: true } button) {
            orig(self, affectedElement);
            return;
        }

        var overridePickedColor = button.GetType().GetField("_overridePickedColor", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = (Color?) overridePickedColor?.GetValue(button) ?? default;

        if (value != Color.Gray) {
            orig(self, affectedElement);
            return;
        }

        DisplayMouseTextIfHovered(affectedElement, "Mods.AomojiResourcePacks.UI.ResourcePacks.ForceEnabled");
    }

    private static void DisplayMouseTextIfHovered(UIElement affectedElement, string textKey) {
        if (!affectedElement.IsMouseHovering)
            return;

        var textValue = Language.GetTextValue(textKey);
        Main.instance.MouseText(textValue);
    }

    private static void DrawModdedIcon(On_UIResourcePack.orig_DrawSelf orig, UIResourcePack self, SpriteBatch spriteBatch) {
        orig(self, spriteBatch);

        if (self.ResourcePack is not ContentSourceResourcePack)
            return;

        var dimensions = self.GetDimensions();
        var asset = ModContent.Request<Texture2D>("AomojiResourcePacks/Assets/UI/ModdedResourcePackIcon").Value;
        spriteBatch.Draw(asset, new Vector2(dimensions.X + dimensions.Width - asset.Width - 3f, dimensions.Y + 2f), asset.Frame(), Color.White);
    }

    private static void UpdateInUseResourcePacks(On_AssetSourceController.orig_UseResourcePacks orig, AssetSourceController self, ResourcePackList resourcePacks) {
        orig(self, resourcePacks);

        if (!actingUponModSourceController) {
            actingUponModSourceController = true;
            // Make mods' asset repositories aware of the resource packs.
            modSourceControllers ??= ModLoader.Mods.ToDictionary(x => x, x => new AssetSourceController(x.Assets, new[] { x.RootContentSource }));

            foreach (var mod in ModLoader.Mods) {
                var controller = modSourceControllers[mod];
                controller.UseResourcePacks(resourcePacks);
                controller.Refresh();
            }

            actingUponModSourceController = false;
        }

        if (!resourcePacksSetThisSession)
            return;

        // Save in-use resource packs.
        enabledResourcePacks = GetPackStatesFromSourceController();
        File.WriteAllText(ResourcePacksPath, JsonConvert.SerializeObject(enabledResourcePacks));
    }
}
