using System;
using System.IO;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content.Sources;
using Terraria.IO;

namespace AomojiVanity.API.ResourcePacks;

public static class ResourcePackLoader {
    private class ModResourcePackData {
        public bool IsModded { get; set; }

        public IContentSource? ContentSource { get; set; }
        
        public IContentSource? RootSource { get; set; }
    }

    private static ConditionalWeakTable<ResourcePack, ModResourcePackData> dataDict = new ();

    public static void Register(ModResourcePack resourcePack) {
        var data = dataDict.GetOrCreateValue(resourcePack.Entity);
        data.IsModded = true;
        data.ContentSource = resourcePack.MakeContentSource();
        data.RootSource = resourcePack.MakeRootSource();
    }

    internal static void Load() {
        dataDict = new ConditionalWeakTable<ResourcePack, ModResourcePackData>();

        IL_ResourcePack.ctor += RegisterResourcePackAsModded;
        On_ResourcePack.HasFile += ResourcePackHasModdedFile;
        On_ResourcePack.OpenStream += ResourcePackOpenModdedStream;
        On_ResourcePack.GetContentSource += ResourcePackGetModdedContentSource;
    }

    internal static void Unload() {
        dataDict = null!;

        IL_ResourcePack.ctor -= RegisterResourcePackAsModded;
        On_ResourcePack.HasFile -= ResourcePackHasModdedFile;
        On_ResourcePack.OpenStream -= ResourcePackOpenModdedStream;
        On_ResourcePack.GetContentSource -= ResourcePackGetModdedContentSource;
    }

    private static void RegisterResourcePackAsModded(ILContext il) {
        var c = new ILCursor(il);
        var label = c.DefineLabel();

        c.GotoNext(x => x.MatchStfld<ResourcePack>(nameof(ResourcePack.IsCompressed)));
        c.GotoPrev(MoveType.Before, x => x.MatchLdarg(0));
        c.MarkLabel(label);

        c.GotoPrev(MoveType.Before, x => x.MatchLdarg(2));
        c.Emit(OpCodes.Ldarg_2);
        c.Emit(OpCodes.Brfalse, label);
    }

    private static bool ResourcePackHasModdedFile(On_ResourcePack.orig_HasFile orig, ResourcePack self, string fileName) {
        var data = dataDict.GetOrCreateValue(self);
        if (!data.IsModded || data.RootSource is null)
            return orig(self, fileName);

        // Modded resource packs rely on a provided content source.
        return data.RootSource.HasAsset(fileName);
    }

    private static Stream ResourcePackOpenModdedStream(On_ResourcePack.orig_OpenStream orig, ResourcePack self, string filename) {
        var data = dataDict.GetOrCreateValue(self);
        if (!data.IsModded || data.RootSource is null)
            return orig(self, filename);

        // Modded resource packs rely on a provided content source.
        return data.RootSource.OpenStream(filename);
    }
    
    private static IContentSource ResourcePackGetModdedContentSource(On_ResourcePack.orig_GetContentSource orig, ResourcePack self) {
        var data = dataDict.GetOrCreateValue(self);
        if (!data.IsModded || data.ContentSource is null)
            return orig(self);

        // Modded resource packs rely on a provided content source.
        return data.ContentSource;
    }
}
