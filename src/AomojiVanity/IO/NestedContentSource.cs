using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReLogic.Content;
using ReLogic.Content.Sources;

namespace AomojiVanity.IO; 

/// <summary>
///     A wrapper around an <see cref="IContentSource"/> instance for nested
///     paths.
///     <br />
///     Also adds support for arbitrary extensions.
/// </summary>
public class NestedContentSource : IContentSource {
    IContentValidator IContentSource.ContentValidator {
        get => source.ContentValidator;
        set => source.ContentValidator = value;
    }

    RejectedAssetCollection IContentSource.Rejections => source.Rejections;
    
    private readonly IContentSource source;
    private readonly string path;
    private readonly bool arbitraryExtensions;
    
    public NestedContentSource(IContentSource source, string path, bool arbitraryExtensions) {
        this.source = source;
        this.path = path;
        this.arbitraryExtensions = arbitraryExtensions;
    }
    
    IEnumerable<string> IContentSource.EnumerateAssets() {
        return source.EnumerateAssets().Where(asset => asset.StartsWith(path));
    }

    string IContentSource.GetExtension(string assetName) {
        return arbitraryExtensions ? Path.GetExtension(assetName) : source.GetExtension(path + assetName);
    }

    Stream IContentSource.OpenStream(string fullAssetName) {
        return source.OpenStream(path + fullAssetName);
    }
}
