using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AomojiVanity.API.Hair;

// TODO: Save even when uhh mod is unloaded?
/// <summary>
///     Saves and loads hair data for a player.
/// </summary>
internal sealed class ModHairPlayer : ModPlayer {
    private const string current_hair_id_key = "CurrentHairId";

    public override void SaveData(TagCompound tag) {
        base.SaveData(tag);

        var modHair = HairLoader.GetHair(Player.hair);

        if (modHair != null)
            tag.Add(current_hair_id_key, modHair.FullName);
    }

    public override void LoadData(TagCompound tag) {
        base.LoadData(tag);

        if (!tag.ContainsKey(current_hair_id_key))
            return;

        var modHairKey = tag.GetString(current_hair_id_key);
        ModContent.SplitName(modHairKey, out var modName, out var hairName);

        if (!ModLoader.TryGetMod(modName, out var modInstance))
            return;

        var modHair = modInstance.GetContent<ModHair>().FirstOrDefault(x => x.Name == hairName);
        if (modHair == null)
            return;

        Player.hair = modHair.Type;
    }
}
