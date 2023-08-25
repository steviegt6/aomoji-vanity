using ReLogic.Content.Sources;
using Terraria;
using Terraria.IO;

namespace AomojiResourcePacks.API;

/// <summary>
///     A <see cref="ResourcePack"/> which uses <see cref="IContentSource"/>s
///     to load content. Also contains a reference to a source
///     <see cref="ModResourcePack"/>.
/// </summary>
/// <remarks>
///     Due to the nature of <see cref="ResourcePack"/>, hardcoded patches have
///     to be applied with MonoMod. See <see cref="ResourcePackLoader"/> for
///     these patches.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global - instantiated, just
// through FormatterServices and a direct constructor call through reflection.
public class ContentSourceResourcePack : ResourcePack {
    /// <summary>
    ///     The <see cref="IContentSource"/> used to lo
    /// </summary>
    public IContentSource? ContentSource { get; set; }

    /// <summary>
    ///     The <see cref="IContentSource"/> used to load special resource pack
    ///     files, such as <c>pack.json</c> and <c>icon.png</c>.
    /// </summary>
    public IContentSource? RootSource { get; set; }
    
    /// <summary>
    ///     The <see cref="ModResourcePack"/> that this resource pack is
    ///     associated with.
    /// </summary>
    public ModResourcePack? ModResourcePack { get; set; }

    /// <summary>
    ///     Constructs a new instance of <see cref="ContentSourceResourcePack"/>
    ///     with <see cref="Main.Services"/>, <see langword="null"/> path, and
    ///     no branding.
    /// </summary>
    public ContentSourceResourcePack() : base(Main.instance.Services, null) { }
}
