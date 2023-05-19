using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AomojiVanity.Content.Features.MiscVanity; 

public sealed class MiscVanitySlotPlayer : ModPlayer {
    public Item[] MiscVanity = new Item[5];

    public override void Initialize() {
        base.Initialize();

        MiscVanity = new Item[] { new(), new(), new(), new(), new() };
    }

    public override void SaveData(TagCompound tag) {
        base.SaveData(tag);

        var miscVanity = MiscVanity.Select(ItemIO.Save).ToList();
        tag.Add("MiscVanity", miscVanity);
    }

    public override void LoadData(TagCompound tag) {
        base.LoadData(tag);
        
        var miscVanity = tag.GetList<TagCompound>("MiscVanity");
        for (var i = 0; i < MiscVanity.Length; i++)
            MiscVanity[i] = ItemIO.Load(miscVanity[i]);
    }
}
