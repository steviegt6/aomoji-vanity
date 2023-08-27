using AomojiCommonLibs.Reflection.RuntimeAccessor;
using Microsoft.Xna.Framework;
using ReLogic.Threading;
using Terraria;
using Terraria.Graphics.Light;

namespace Hikari.Content.Lighting;

public class HikariTileLightScanner : TileLightScanner {
    private interface ITileLightScannerAccessor {
        [FieldAccessor("_drawInvisibleWalls")]
        bool DrawInvisibleWalls { get; set; }

        [MethodAccessor("GetTileMask")]
        LightMaskMode GetTileMask(Tile tile);
    }

    private readonly ITileLightScannerAccessor accessor;

    public HikariTileLightScanner() {
        accessor = RuntimeAccessorGenerator.GenerateAccessor<TileLightScanner, ITileLightScannerAccessor>(this);
    }

    public void ExportTo(Rectangle area, HikariLightMap outputMap, TileLightScannerOptions options) {
        accessor.DrawInvisibleWalls = options.DrawInvisibleWalls;

        var worldBounds = new Rectangle(1, 1, Main.maxTilesX - 1, Main.maxTilesY - 1);
        area = Rectangle.Intersect(area, worldBounds);
        FastParallel.For(
            area.Left,
            area.Right,
            (start, end, _) => {
                for (var x = start; x < end; ++x)
                for (var y = area.Top; y < area.Bottom; ++y) {
                    /*if (FastIsTileNullOrTouchingNull(x, y)) {
                        outputMap.SetMaskAt(x, y, LightMaskMode.None);
                        outputMap[x - area.X, y - area.Y] = Vector3.Zero;
                    }*/
                    //else {
                    var tileMask = accessor.GetTileMask(Main.tile[x, y]);
                    outputMap.SetMaskAt(x - area.X, y - area.Y, tileMask);
                    GetTileLight(x, y, out var color);
                    outputMap[x - area.X, y - area.Y] = color;
                    //}
                }
            }
        );
    }

    // OPTIMIZATION: Remove null checks from the original method. In reality,
    // this likely accomplishes nothing.
    private static bool FastIsTileNullOrTouchingNull(int x, int y) {
        return !WorldGen.InWorld(x, y, 1);
    }
}
