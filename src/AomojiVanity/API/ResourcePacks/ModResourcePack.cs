using AomojiVanity.IO.ContentSources;
using AomojiVanity.IO.Serialization;
using ReLogic.Content.Sources;
using Terraria.IO;
using Terraria.ModLoader;

namespace AomojiVanity.API.ResourcePacks;

/// <summary>
///     A modded resource pack. That is, a resource pack provided directly by
///     a mod.
/// </summary>
public abstract class ModResourcePack : ModType<ResourcePack> {
#region ModType Impl
    protected sealed override void Register() {
        ResourcePackLoader.Register(this);
    }

    protected override ResourcePack CreateTemplateEntity() {
        // Get an uninitialized (unconstructed) instance of
        // ContentSourceResourcePack so we can set properties prior to actually
        // calling the constructor. The constructor invokes a method that
        // requires these properties to be set (defined in vanilla code that we
        // edit) so we have to do this.
        var pack = FormatterUtilities.GetUninitializedObject<ContentSourceResourcePack>();
        pack.ContentSource = MakeContentSource();
        pack.RootSource = MakeRootSource();
        pack.InitializeObject(); // Invokes `.ctor()` (parameterless).
        pack.IsEnabled = DefaultEnableState;

        return pack;
    }
#endregion

    /// <summary>
    ///     The root path of this resource pack. This is the path that is used
    ///     as a root for resource resolution.
    /// </summary>
    public abstract string RootPath { get; }

    /// <summary>
    ///     The default enabled/disabled state of this resource pack.
    /// </summary>
    public virtual bool DefaultEnableState => false;

    /// <summary>
    ///     Creates the <see cref="IContentSource"/> used for getting resources.
    /// </summary>
    /// <returns>TODO</returns>
    public virtual IContentSource MakeContentSource() {
        return new ModFileContentSourceWithRoot(Mod, NormalizeAndAppendSeparator(RootPath + "/Content"));
    }

    /// <summary>
    ///     Creates the <see cref="IContentSource"/> used for getting meta
    ///     resources.
    /// </summary>
    /// <returns>TODO</returns>
    public virtual IContentSource MakeRootSource() {
        return new ModFileContentSourceWithRoot(Mod, NormalizeAndAppendSeparator(RootPath));
    }

    private static string NormalizeAndAppendSeparator(string path) {
        path = path.Replace('\\', '/');
        if (!path.EndsWith('/'))
            path += '/';
        return path;
    }
}
