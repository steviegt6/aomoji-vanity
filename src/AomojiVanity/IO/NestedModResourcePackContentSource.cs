using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ReLogic.Content;
using ReLogic.Content.Sources;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace AomojiVanity.IO;

public class NestedModResourcePackContentSource : IContentSource {
    IContentValidator IContentSource.ContentValidator { get; set; } = VanillaContentValidator.Instance;

    RejectedAssetCollection IContentSource.Rejections { get; } = new();

    private readonly TmodFile file;
    private readonly string path;

    public NestedModResourcePackContentSource(Mod mod, string path) {
        file = (TmodFile) typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mod)!;
        this.path = path;
    }

    private string ExpandAndSanitizePath(string assetPath) {
        return path + assetPath.Replace('\\', '/');
    }

    private string? GetPathWithExtensionFromFile(string assetPath) {
        if (Path.GetExtension(assetPath) != string.Empty)
            return assetPath;

        return file.GetFileNames().FirstOrDefault(x => x.StartsWith(assetPath) && x[assetPath.Length] == '.');
    }

    public IEnumerable<string> EnumerateAssets() {
        return file.GetFileNames().Where(asset => asset.StartsWith(path)).Select(asset => Path.GetFileNameWithoutExtension(asset[path.Length..]));
    }

    string? IContentSource.GetExtension(string assetName) {
        return Path.GetExtension(GetPathWithExtensionFromFile(ExpandAndSanitizePath(assetName)));
    }

    Stream IContentSource.OpenStream(string fullAssetName) {
        return file.GetStream(GetPathWithExtensionFromFile(ExpandAndSanitizePath(fullAssetName)));
    }

    bool IContentSource.HasAsset(string assetName) {
        if (assetName.Contains("Player"))
            ;
        return file.HasFile(GetPathWithExtensionFromFile(ExpandAndSanitizePath(assetName)) ?? assetName);
    }

    IEnumerable<string> IContentSource.GetAllAssetsStartingWith(string assetNameStart) {
        return EnumerateAssets().Where(asset => asset.StartsWith(ExpandAndSanitizePath(assetNameStart)));
    }
}
