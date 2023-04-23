using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReLogic.Content;
using ReLogic.Content.Sources;

namespace AomojiVanity.IO; 

/// <summary>
///     A wrapper around an <see cref="IContentSource"/> instance for nested
///     paths.
/// </summary>
public class NestedContentSource : IContentSource {
    IContentValidator IContentSource.ContentValidator {
        get => source.ContentValidator;
        set => source.ContentValidator = value;
    }

    RejectedAssetCollection IContentSource.Rejections => source.Rejections;
    
    private readonly IContentSource source;
    private readonly string path;
    
    public NestedContentSource(IContentSource source, string path) {
        this.source = source;
        this.path = path;
    }
    
    IEnumerable<string> IContentSource.EnumerateAssets() {
        return source.EnumerateAssets().Where(asset => asset.StartsWith(path));
    }

    string IContentSource.GetExtension(string assetName) {
        return source.GetExtension(path + assetName);
    }

    Stream IContentSource.OpenStream(string fullAssetName) {
        return source.OpenStream(path + fullAssetName);
    }
}
