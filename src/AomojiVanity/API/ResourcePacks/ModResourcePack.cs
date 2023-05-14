using System;
using System.Runtime.Serialization;
using AomojiVanity.IO.ContentSources;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace AomojiVanity.API.ResourcePacks;

public abstract class ModResourcePack : ModType<ResourcePack> {
    internal class ExtendedResourcePack : ResourcePack {
        public IContentSource? ContentSource { get; set; }

        public IContentSource? RootSource { get; set; }

        public ExtendedResourcePack() : base(Main.instance.Services, null) { }
    }

    protected sealed override void Register() {
        ResourcePackLoader.Register(this);
    }

    protected override ResourcePack CreateTemplateEntity() {
        var pack = (ExtendedResourcePack) FormatterServices.GetUninitializedObject(typeof(ExtendedResourcePack));
        pack.ContentSource = MakeContentSource();
        pack.RootSource = MakeRootSource();
        typeof(ExtendedResourcePack).GetConstructor(Type.EmptyTypes)!.Invoke(pack, null);
        return pack;
    }

    protected abstract string RootPath { get; }

    protected virtual IContentSource MakeContentSource() {
        return new ModFileContentSourceWithRoot(Mod, NormalizeAndAppendSeparator(RootPath + "/Content"));
    }

    protected virtual IContentSource MakeRootSource() {
        return new ModFileContentSourceWithRoot(Mod, NormalizeAndAppendSeparator(RootPath));
    }

    private static string NormalizeAndAppendSeparator(string path) {
        path = path.Replace('\\', '/');
        if (!path.EndsWith('/'))
            path += '/';
        return path;
    }
}
