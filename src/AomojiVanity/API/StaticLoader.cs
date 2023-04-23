using AomojiVanity.API.Hair;
using AomojiVanity.API.Hijacking;
using AomojiVanity.API.ResourcePacks;
using Terraria.ModLoader;

namespace AomojiVanity.API;

/// <summary>
///     Centralized loading and unloading for static API classes we sadly cannot
///     use tModLoader's singleton loading system for.
/// </summary>
internal sealed class StaticLoader : ModSystem {
    public override void OnModLoad() {
        base.OnModLoad();

        HairLoader.Load();
        ResourcePackLoader.Load();
    }

    public override void SetupContent() {
        base.SetupContent();

        HairLoader.ResizeArrays(unloading: false);
    }

    public override void OnModUnload() {
        base.OnModUnload();

        HairLoader.ResizeArrays(unloading: true);

        HairLoader.Unload();
        ModHijackLoader.Unload();
        ResourcePackLoader.Unload();
    }
}
