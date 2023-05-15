using ReLogic.Content.Sources;

namespace AomojiVanity.IO.ContentSources;

/// <summary>
///     Indicates an <see cref="IContentSource"/> has a <see cref="Root"/>.
/// </summary>
public interface IHasRoot {
    /// <summary>
    ///     The root of the <see cref="IContentSource"/>.
    /// </summary>
    string Root { get; set; }
}
