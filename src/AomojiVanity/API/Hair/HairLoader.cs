using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;

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
        playerHairCache = TextureAssets.PlayerHair.ToArray();
        playerHairAltCache = TextureAssets.PlayerHairAlt.ToArray();
    }

    internal static void Unload() {
        HairCount = HairID.Count;
    }

    internal static void ResizeArrays(bool unloading) {
        if (unloading) {
            TextureAssets.PlayerHair = playerHairCache;
            TextureAssets.PlayerHairAlt = playerHairAltCache;
            playerHairCache = null;
            playerHairAltCache = null;
        }
        else {
            Array.Resize(ref TextureAssets.PlayerHair, HairCount);
            Array.Resize(ref TextureAssets.PlayerHairAlt, HairCount);
            
            for (var i = HairID.Count; i < HairCount; i++) {
                var hair = GetHair(i)!;

                TextureAssets.PlayerHair[i] = hair.HairTexture;
                TextureAssets.PlayerHairAlt[i] = hair.HairAltTexture;
            }
        }
    }
}
