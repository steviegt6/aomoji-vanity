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

        public override Gender RandomizedCharacterCreationGender { get; }

        public AutoloadedHair(string key, Gender gender) {
            Name = key;
            RandomizedCharacterCreationGender = gender;
        }
    }

    private const string root = "AomojiVanity/Assets/Hair/";
    private readonly string[] unspecified_keys = { };
    private readonly string[] male_keys = { };
    private readonly string[] female_keys = { "Kobayashi", "Ryo" };

    public override void OnModLoad() {
        base.OnModLoad();

        foreach (var key in unspecified_keys)
            Mod.AddContent(new AutoloadedHair(key, Gender.Unspecified));

        foreach (var key in male_keys)
            Mod.AddContent(new AutoloadedHair(key, Gender.Male));

        foreach (var key in female_keys)
            Mod.AddContent(new AutoloadedHair(key, Gender.Female));
    }
}
