using AomojiVanity.API.Hair;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AomojiVanity.Content.Hair;

/// <summary>
///     Autoloads hairs.
/// </summary>
internal sealed class HairAutoloader : ModSystem {
    private class AutoloadedHair : ModHair {
        public override Asset<Texture2D> HairTexture => ModContent.Request<Texture2D>(root + key);

        public override Asset<Texture2D> HairAltTexture => ModContent.Request<Texture2D>(root + key + "Alt");

        private readonly string key;

        public AutoloadedHair(string key) {
            this.key = key;
        }
    }

    private const string root = "AomojiVanity/Assets/Hair/";
    private readonly string[] keys = { "Kobayashi", "Ryo" };

    public override void OnModLoad() {
        base.OnModLoad();

        foreach (var key in keys)
            Mod.AddContent(new AutoloadedHair(key));
    }
}
