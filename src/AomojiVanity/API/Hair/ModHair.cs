using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hair;

/// <summary>
///     A modded piece of hair.
/// </summary>
public abstract class ModHair : ModType {
    public int Type { get; internal set; }

    public bool DrawBackHair {
        get => HairID.Sets.DrawBackHair[Type];
        set => HairID.Sets.DrawBackHair[Type] = value;
    }

    public abstract Asset<Texture2D> HairTexture { get; }

    public abstract Asset<Texture2D> HairAltTexture { get; }

    public virtual bool IsUnlocked(bool isAtStylist, bool isAtCharacterCreation) {
        return true;
    }

    protected sealed override void Register() {
        HairLoader.Register(this);
        ModTypeLookup<ModHair>.Register(this);
    }
}
