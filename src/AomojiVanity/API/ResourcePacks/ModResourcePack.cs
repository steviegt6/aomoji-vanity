using AomojiVanity.IO;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace AomojiVanity.API.ResourcePacks;

public abstract class ModResourcePack : ModType<ResourcePack> {
    protected sealed override void Register() {
        ResourcePackLoader.Register(this);
    }

    protected override ResourcePack CreateTemplateEntity() {
        return new ResourcePack(Main.instance.Services, null);
    }

    protected abstract string RootPath { get; }

    public virtual IContentSource MakeContentSource() {
        return new NestedContentSource(Mod.RootContentSource, NormalizeAndAppendSeparator(RootPath + "/Content"));
    }

    public virtual IContentSource MakeRootSource() {
        return new NestedContentSource(Mod.RootContentSource, NormalizeAndAppendSeparator(RootPath));
    }

    private static string NormalizeAndAppendSeparator(string path) {
        path = path.Replace('\\', '/');
        if (!path.EndsWith('/'))
            path += '/';
        return path;
    }
}
