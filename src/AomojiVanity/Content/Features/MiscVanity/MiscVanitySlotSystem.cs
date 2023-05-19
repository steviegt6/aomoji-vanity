using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace AomojiVanity.Content.Features.MiscVanity;

public sealed class MiscVanitySlotSystem : ModSystem {
    public override void Load() {
        base.Load();

        // Technically speaking, we should be using
        // ModSystem::ModifyInterfaceLayers to draw this added UI, but I don't
        // really care? It's kind of a hassle and I want to ensure these are
        // drawn as close to the inventory as possible. Sorry, not sorry.
        On_Main.DrawInventory += DrawMiscVanitySlots;
    }

    // TODO: Mostly ripped from vanilla; clean this up?
    private static void DrawMiscVanitySlots(On_Main.orig_DrawInventory orig, Main self) {
        orig(self);

        // TODO: For funsies - ID/enum?
        // EquipPage 2 is the misc. slots page.
        if (Main.EquipPage != 2)
            return;

        var oldScale = Main.inventoryScale;
        Main.inventoryScale = 0.85f;
        var mousePos = Main.MouseScreen.ToPoint();
        var panelRect = new Rectangle(0, 0, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));

        var inv = Main.LocalPlayer.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity;
        var drawX = Main.screenWidth - 92;
        var drawY = GetMh() + 174;

        panelRect.X = drawX + (2 * -47);

        for (var i = 0; i < 5; i++) {
            // Disabling some slots for now...
            if (i is 0 or 1 or 3) // pet, light pet, and mount
                continue;
            
            var context = i switch {
                0 => 19, // pet
                1 => 20, // light pet
                2 => 18, // minecart
                3 => 17, // mount
                4 => 16, // hook
                _ => 0,
            };

            panelRect.Y = drawY + (i * 47);

            if (panelRect.Contains(mousePos) && !PlayerInput.IgnoreMouseInterface) {
                Main.LocalPlayer.mouseInterface = true;
                Main.armorHide = true;
                ItemSlot.Handle(inv, context, i);
            }

            ItemSlot.Draw(Main.spriteBatch, inv, context, i, panelRect.TopLeft());
        }
        
        Main.inventoryScale = oldScale;
    }

    // TODO: Replace with Main::mH once we use collate.
    private static int GetMh() {
        var mh = 0;

        if (!Main.mapEnabled)
            return mh;

        if (!Main.mapFullscreen && Main.mapStyle == 1)
            mh = 256;

        // PlayerInput::SetZoom_UI() is normally called before here, don't care.
        // This runs after Main::DrawInterface_16_MapOrMinimap(), which is where
        // this is taken from, so it's fine anyway.
        if (mh + Main.instance.RecommendedEquipmentAreaPushUp > Main.screenHeight)
            return Main.screenHeight - Main.instance.RecommendedEquipmentAreaPushUp;

        return mh;
    }
}
