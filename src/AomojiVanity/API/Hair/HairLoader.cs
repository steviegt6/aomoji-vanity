using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AomojiVanity.API.Hair;

public static class HairLoader {
    public static int HairCount { get; private set; }

    private static List<ModHair> hairs = new ();

    private static Asset<Texture2D>[]? playerHairCache;
    private static Asset<Texture2D>[]? playerHairAltCache;

    public static void Register(ModHair hair) {
        hair.Type = HairCount++;
        hairs.Add(hair);
    }

    public static ModHair? GetHair(int type) {
        return type < HairID.Count || type >= HairCount ? null : hairs[type - HairID.Count];
    }

    internal static void Load() {
        HairCount = HairID.Count;

        if (!Main.dedServ) {
            playerHairCache = TextureAssets.PlayerHair.ToArray();
            playerHairAltCache = TextureAssets.PlayerHairAlt.ToArray();
        }

        On_HairstyleUnlocksHelper.RebuildList += AddUnlocksForModHairs;
        On_HairstyleUnlocksHelper.ListWarrantsRemake += AlwaysWarrantsRemake;
    }

    internal static void Unload() {
        HairCount = HairID.Count;

        On_HairstyleUnlocksHelper.RebuildList -= AddUnlocksForModHairs;
        On_HairstyleUnlocksHelper.ListWarrantsRemake -= AlwaysWarrantsRemake;
    }

    private static void AddUnlocksForModHairs(On_HairstyleUnlocksHelper.orig_RebuildList orig, HairstyleUnlocksHelper self) {
        orig(self);

        var isAtStylist = Main.hairWindow && !Main.gameMenu;
        var isAtCharacterCreation = Main.gameMenu;

        foreach (var hair in hairs) {
            if (hair.IsUnlocked(isAtStylist, isAtCharacterCreation)) {
                ModContent.GetInstance<AomojiVanity>().Logger.Debug($"Hair {hair.Name} is unlocked.");
                self.AvailableHairstyles.Add(hair.Type);
            }
        }
    }

    private static bool AlwaysWarrantsRemake(On_HairstyleUnlocksHelper.orig_ListWarrantsRemake orig, HairstyleUnlocksHelper self) {
        // We need to update conditions still.
        orig(self);

        // TODO: Performance implications...
        return true;
    }

    internal static void ResizeArrays(bool unloading) {
        if (Main.dedServ)
            return;

        if (unloading) {
            TextureAssets.PlayerHair = playerHairCache;
            TextureAssets.PlayerHairAlt = playerHairAltCache;
            playerHairCache = null;
            playerHairAltCache = null;
        }
        else {
            Array.Resize(ref TextureAssets.PlayerHair, HairCount);
            Array.Resize(ref TextureAssets.PlayerHairAlt, HairCount);
            Array.Resize(ref HairID.Sets.DrawBackHair, HairCount);

            for (var i = HairID.Count; i < HairCount; i++) {
                var hair = GetHair(i)!;

                TextureAssets.PlayerHair[i] = hair.HairTexture;
                TextureAssets.PlayerHairAlt[i] = hair.HairAltTexture;
            }

            Main.Hairstyles.UpdateUnlocks();
        }
    }
}
