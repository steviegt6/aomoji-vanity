using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ReLogic.Content;
using ReLogic.Content.Sources;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace AomojiVanity.IO.ContentSources;

/// <summary>
///     An <see cref="IContentSource"/> implementation that uses a
///     <see cref="TmodFile"/> as it source, and supports a root path which is
///     prepended to all asset requests.
/// </summary>
public class ModFileContentSourceWithRoot : IContentSource {
    IContentValidator IContentSource.ContentValidator { get; set; } = VanillaContentValidator.Instance;

    RejectedAssetCollection IContentSource.Rejections { get; } = new();

    private readonly TmodFile file;
    private readonly string root;

    public ModFileContentSourceWithRoot(Mod mod, string root) {
        file = (TmodFile) typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mod)!;
        this.root = root;
    }

    private string ExpandAndSanitizePath(string assetPath) {
        return root + assetPath.Replace('\\', '/');
    }

    private string? GetPathWithExtensionFromFile(string assetPath) {
        if (Path.GetExtension(assetPath) != string.Empty)
            return assetPath;

        return file.GetFileNames().FirstOrDefault(x => x.StartsWith(assetPath) && x[assetPath.Length] == '.');
    }

    public IEnumerable<string> EnumerateAssets() {
        return file.GetFileNames().Where(asset => asset.StartsWith(root)).Select(asset => Path.GetFileNameWithoutExtension(asset[root.Length..]));
    }

    string? IContentSource.GetExtension(string assetName) {
        return Path.GetExtension(GetPathWithExtensionFromFile(ExpandAndSanitizePath(assetName)));
    }

    Stream IContentSource.OpenStream(string fullAssetName) {
        return file.GetStream(GetPathWithExtensionFromFile(ExpandAndSanitizePath(fullAssetName)));
    }

    bool IContentSource.HasAsset(string assetName) {
        return file.HasFile(GetPathWithExtensionFromFile(ExpandAndSanitizePath(assetName)) ?? assetName);
    }

    IEnumerable<string> IContentSource.GetAllAssetsStartingWith(string assetNameStart) {
        return EnumerateAssets().Where(asset => asset.StartsWith(ExpandAndSanitizePath(assetNameStart)));
    }
}
