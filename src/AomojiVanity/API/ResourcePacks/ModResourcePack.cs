using System;
using AomojiVanity.IO;
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
        return new ExtendedResourcePack {
            ContentSource = MakeContentSource(),
            RootSource = MakeRootSource(),
        };
    }

    protected abstract string RootPath { get; }

    protected virtual IContentSource MakeContentSource() {
        return new NestedContentSource(Mod.RootContentSource, NormalizeAndAppendSeparator(RootPath + "/Content"));
    }

    protected virtual IContentSource MakeRootSource() {
        return new NestedContentSource(Mod.RootContentSource, NormalizeAndAppendSeparator(RootPath));
    }

    private static string NormalizeAndAppendSeparator(string path) {
        path = path.Replace('\\', '/');
        if (!path.EndsWith('/'))
            path += '/';
        return path;
    }
}
