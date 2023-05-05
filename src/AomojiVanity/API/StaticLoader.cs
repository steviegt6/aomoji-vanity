using AomojiVanity.API.Hijacking;
using AomojiVanity.API.ResourcePacks;
using Terraria.ModLoader;

namespace AomojiVanity.API;

/// <summary>
///     Centralized loading and unloading for static API classes we sadly cannot
///     use tModLoader's singleton loading system for.
/// </summary>
internal sealed class StaticLoader : ModSystem {
    public StaticLoader() {
        ResourcePackLoader.Load();
    }

    public override void SetupContent() {
        base.SetupContent();
    }

    public override void OnModUnload() {
        base.OnModUnload();

        ModHijackLoader.Unload();
        ResourcePackLoader.Unload();
    }
}
