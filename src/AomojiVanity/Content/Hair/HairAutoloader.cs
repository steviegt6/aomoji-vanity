using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AomojiVanity.Content.Hair;

/// <summary>
///     Autoloads hairs.
/// </summary>
internal sealed class HairAutoloader : ModSystem {
    private class AutoloadedHair : ModHair {
        public override string Name { get; }

        public override string Texture => "AomojiVanity/Assets/Hair/" + Name;

        public override bool IsMale { get; }

        public AutoloadedHair(string key, bool isMale) {
            Name = key;
            IsMale = isMale;
        }
    }

    private const string root = "AomojiVanity/Assets/Hair/";
    private readonly string[] female_keys = { "Kobayashi", "Ryo" };
    private readonly string[] male_keys = { };

    public override void OnModLoad() {
        base.OnModLoad();

        foreach (var key in female_keys)
            Mod.AddContent(new AutoloadedHair(key, false));

        foreach (var key in male_keys)
            Mod.AddContent(new AutoloadedHair(key, true));
    }
}
