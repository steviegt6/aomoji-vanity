using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AomojiVanity.Content.Features.MiscVanity;

public sealed class MiscVanitySlotSystem : ModSystem {
    private static bool drawMountDust = true;

    public override void Load() {
        base.Load();

        // Technically speaking, we should be using
        // ModSystem::ModifyInterfaceLayers to draw this added UI, but I don't
        // really care? It's kind of a hassle and I want to ensure these are
        // drawn as close to the inventory as possible. Sorry, not sorry.
        On_Main.DrawInventory += DrawMiscVanitySlots;
        IL_Main.DrawInventory += OffsetMiscDyeSlots;

        On_Mount.Draw += DrawVanityCart;
        On_Mount.DoSpawnDust += DontSpawnDustWhenSettingVanityCart;
        On_Mount.UpdateEffects += UpdateEffectsForVanityCart;
        On_Player.Update += UseVanityCartDelegations;

        On_Main.DrawProjDirect += DrawVanityGrapplingHook;
    }

#region Misc. Inventory Menu Drawing
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

        panelRect.X = drawX + (1 /*2*/ * -47);

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

    private static void OffsetMiscDyeSlots(ILContext il) {
        var c = new ILCursor(il);

        var l = -1;
        c.GotoNext(MoveType.After, x => x.MatchLdcI4(-47));
        c.GotoPrev(x => x.MatchLdloc(out l));

        // lol names are hard
        if (l == -1)
            throw new InvalidOperationException("Failed to find `l`!");

        c.GotoNext(MoveType.After, x => x.MatchMul());
        c.Emit(OpCodes.Ldloc, l);
        c.EmitDelegate((int lVal) => lVal == 1 ? 47 : 0);
        c.Emit(OpCodes.Sub);
    }
#endregion

#region Vanity Mount Detours
    private static void DrawVanityCart(On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 position, Color drawColor, SpriteEffects playerEffect, float shadow) {
        if (!drawPlayer.mount.Cart) {
            orig(self, playerDrawData, drawType, drawPlayer, position, drawColor, playerEffect, shadow);
            return;
        }

        var mount = drawPlayer.miscEquips[2];
        var vanityMount = drawPlayer.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity[2];

        if (vanityMount.IsAir) {
            orig(self, playerDrawData, drawType, drawPlayer, position, drawColor, playerEffect, shadow);
            return;
        }

        // No reason to behave differently if the mount is the same, right?
        if (self._type == vanityMount.mountType) {
            orig(self, playerDrawData, drawType, drawPlayer, position, drawColor, playerEffect, shadow);
            return;
        }

        drawMountDust = false;
        drawPlayer.mount.SetMount(vanityMount.mountType, drawPlayer, drawPlayer.minecartLeft);
        orig(self, playerDrawData, drawType, drawPlayer, position, drawColor, playerEffect, shadow);
        drawPlayer.mount.SetMount(mount.mountType, drawPlayer, drawPlayer.minecartLeft);
        drawMountDust = true;
    }

    private static void DontSpawnDustWhenSettingVanityCart(On_Mount.orig_DoSpawnDust orig, Mount self, Player mountedPlayer, bool isDismounting) {
        if (drawMountDust)
            orig(self, mountedPlayer, isDismounting);
    }

    private static void UpdateEffectsForVanityCart(On_Mount.orig_UpdateEffects orig, Mount self, Player mountedPlayer) {
        // TODO: Figure out how to safely update for visuals only?
        orig(self, mountedPlayer);

        /*if (!self.Cart || self._data is null) {
            orig(self, mountedPlayer);
            return;
        }

        // var mount = mountedPlayer.miscEquips[2];
        var vanityMount = mountedPlayer.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity[2];

        if (vanityMount.IsAir) {
            orig(self, mountedPlayer);
            return;
        }

        // No reason to behave differently if the mount is the same, right?
        if (self._type == vanityMount.mountType) {
            orig(self, mountedPlayer);
            return;
        }

        // This feels... *bad*...
        // Update: it *is* **bad**.
        var oldType = self._type;
        self._type = vanityMount.mountType;

        // No idea if I need to bother with delegations here, ooh...
        var oldDelegations = self._data.delegations;
        var newDelegations = Mount.mounts[vanityMount.mountType].delegations;
        self._data.delegations = newDelegations;
        orig(self, mountedPlayer);
        self._data.delegations = oldDelegations;

        self._type = oldType;*/
    }

    private static void UseVanityCartDelegations(On_Player.orig_Update orig, Player self, int i) {
        if (!self.mount.Cart || self.mount._data is null) {
            orig(self, i);
            return;
        }

        var mount = self.miscEquips[2];
        var vanityMount = self.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity[2];

        if (vanityMount.IsAir) {
            orig(self, i);
            return;
        }

        // No reason to behave differently if the mount is the same, right?
        if (self.mount._type == vanityMount.mountType) {
            orig(self, i);
            return;
        }

        var oldDelegations = self.mount._data.delegations;
        var newDelegations = Mount.mounts[vanityMount.mountType].delegations;
        self.mount._data.delegations = newDelegations;
        orig(self, i);
        self.mount._data.delegations = oldDelegations;
    }
#endregion

#region Vanity Hook Detours
    private static void DrawVanityGrapplingHook(On_Main.orig_DrawProjDirect orig, Main self, Projectile proj) {
        if (proj.aiStyle != ProjAIStyleID.Hook) {
            orig(self, proj);
            return;
        }

        var player = Main.player[proj.owner];

        if (player.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity[4].IsAir) {
            orig(self, proj);
            return;
        }

        var vanityType = player.GetModPlayer<MiscVanitySlotPlayer>().MiscVanity[4].shoot;

        if (proj.type == vanityType) {
            orig(self, proj);
            return;
        }

        // Feels iffy...
        var oldType = proj.type;
        proj.type = vanityType;
        orig(self, proj);
        proj.type = oldType;
    }
#endregion
}
