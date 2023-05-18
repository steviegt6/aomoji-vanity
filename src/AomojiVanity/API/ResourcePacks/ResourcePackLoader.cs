using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Content.Sources;
using ReLogic.Utilities;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AomojiVanity.API.ResourcePacks;

public static class ResourcePackLoader {
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    private sealed class LoaderSystem : ModSystem {
        public LoaderSystem() {
            ResourcePackLoader.Load();
        }

        public override void OnModUnload() {
            base.OnModUnload();

            ResourcePackLoader.Unload();
        }
    }

    private static List<ModResourcePack> modResourcePacks = new();

    public static void Register(ModResourcePack resourcePack) {
        modResourcePacks.Add(resourcePack);
    }

    internal static void Load() {
        modResourcePacks = new List<ModResourcePack>();

        IL_ResourcePack.ctor += SkipPathSettingForModdedPacks;
        On_ResourcePack.HasFile += ResourcePackHasModdedFile;
        On_ResourcePack.OpenStream += ResourcePackOpenModdedStream;
        On_ResourcePack.GetContentSource += ResourcePackGetModdedContentSource;
        On_ResourcePack.CreateIcon += ResourcePackGetRawImgIcon;

        On_ResourcePackList.FromJson += FromJsonAddModdedPacks;
    }

    internal static void Unload() {
        modResourcePacks = null!;
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
        var list = orig(serializedState, services, searchPath);
        return new ResourcePackList(list.AllPacks.Concat(modResourcePacks.Select(x => x.Entity)));
    }
}
