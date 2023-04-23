using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AomojiVanity.API.Hair;

/// <summary>
///     Saves and loads hair data for a player.
/// </summary>
internal sealed class ModHairPlayer : ModPlayer {
    private const string current_hair_id_key = nameof(CurrentHairId);

    public string? CurrentHairId { get; set; }

    public override void SaveData(TagCompound tag) {
        base.SaveData(tag);

        if (CurrentHairId != null)
            tag.Add(current_hair_id_key, CurrentHairId);
    }

    public override void LoadData(TagCompound tag) {
        base.LoadData(tag);

        if (tag.ContainsKey(current_hair_id_key))
            CurrentHairId = tag.GetString(current_hair_id_key);
    }
}
