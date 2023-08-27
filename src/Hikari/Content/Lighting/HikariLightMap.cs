using System.Numerics;
using System.Runtime.CompilerServices;
using ReLogic.Threading;
using Terraria.Graphics.Light;
using Terraria.Utilities;

namespace Hikari.Content.Lighting;

public class HikariLightMap {
    private const int default_width = 203;
    private const int default_height = 203;
    private const int default_size = default_width * default_height;

    private Vector3[] colors = new Vector3[default_size];
    private LightMaskMode[] mask = new LightMaskMode[default_size];
    private FastRandom random = FastRandom.CreateWithRandomSeed();

    public int NonVisiblePadding { get; set; }

    public int Width { get; private set; } = default_width;

    public int Height { get; private set; } = default_height;

    public float LightDecayThroughAir { get; set; } = 0.91f;

    public float LightDecayThroughSolid { get; set; } = 0.56f;

    public Vector3 LightDecayThroughWater { get; set; } = new Vector3(0.88f, 0.96f, 1.015f) * 0.91f;

    public Vector3 LightDecayThroughHoney { get; set; } = new(0.75f, 0.7f, 0.6f);

    public Vector3 this[int x, int y] {
        get => colors[IndexOf(x, y)];
        set => colors[IndexOf(x, y)] = value;
    }

    public void GetLight(int x, int y, out Vector3 color) {
        color = colors[this.IndexOf(x, y)];
    }

    public LightMaskMode GetMask(int x, int y) {
        return mask[IndexOf(x, y)];
    }

    public void Clear() {
        // OPTIMIZATION: FastParallel.For is faster than pretty much any loop
        // variation (including Parallel.For), as well as directly array
        // reinitialization.
        FastParallel.For(
            0,
            colors.Length,
            (start, end, _) => {
                for (var index = start; index < end; ++index) {
                    colors[index] = Vector3.Zero;
                    mask[index] = LightMaskMode.None;
                }
            }
        );
    }

    public void SetMaskAt(int x, int y, LightMaskMode mode) {
        mask[IndexOf(x, y)] = mode;
    }

    public void Blur() {
        BlurPass();
        BlurPass();
        random.NextSeed();
    }

    private void BlurPass() {
        FastParallel.For(
            0,
            Width,
            (start, end, _) => {
                for (var x = start; x < end; x++) {
                    BlurLine(IndexOf(x, 0), IndexOf(x, Height - 1 - NonVisiblePadding), 1);
                    BlurLine(IndexOf(x, Height - 1), IndexOf(x, NonVisiblePadding), -1);
                }
            }
        );
        FastParallel.For(
            0,
            Height,
            (start, end, _) => {
                for (var y = start; y < end; y++) {
                    BlurLine(IndexOf(0, y), IndexOf(Width - 1 - NonVisiblePadding, y), Height);
                    BlurLine(IndexOf(Width - 1, y), IndexOf(NonVisiblePadding, y), -Height);
                }
            }
        );
    }

    private void BlurLine(int startIndex, int endIndex, int stride) {
        var maxColors = Vector3.Zero;

        var decayRed = false;
        var decayGreen = false;
        var decayBlue = false;

        for (var i = startIndex; i != endIndex + stride; i += stride) {
            if (maxColors.X < colors[i].X) {
                maxColors.X = colors[i].X;
                decayRed = false;
            }
            else if (!decayRed) {
                if (maxColors.X < 0.0185)
                    decayRed = true;
                else
                    colors[i].X = maxColors.X;
            }

            if (maxColors.Y < colors[i].Y) {
                maxColors.Y = colors[i].Y;
                decayGreen = false;
            }
            else if (!decayGreen) {
                if (maxColors.Y < 0.0185)
                    decayGreen = true;
                else
                    colors[i].Y = maxColors.Y;
            }

            if (maxColors.Z < colors[i].Z) {
                maxColors.Z = colors[i].Z;
                decayBlue = false;
            }
            else if (!decayBlue) {
                if (maxColors.Z < 0.0185)
                    decayBlue = true;
                else
                    colors[i].Z = maxColors.Z;
            }

            if (!(decayRed & decayGreen & decayBlue)) {
                switch (mask[i]) {
                    case LightMaskMode.None:
                        if (!decayRed)
                            maxColors.X *= LightDecayThroughAir;
                        if (!decayGreen)
                            maxColors.Y *= LightDecayThroughAir;
                        if (!decayBlue)
                            maxColors.Z *= LightDecayThroughAir;
                        break;

                    case LightMaskMode.Solid:
                        if (!decayRed)
                            maxColors.X *= LightDecayThroughSolid;
                        if (!decayGreen)
                            maxColors.Y *= LightDecayThroughSolid;
                        if (!decayBlue)
                            maxColors.Z *= LightDecayThroughSolid;
                        break;

                    case LightMaskMode.Water:
                        var waterDecayFactor = random.WithModifier((ulong)i).Next(98, 100) / 100f;
                        if (!decayRed)
                            maxColors.X *= LightDecayThroughWater.X * waterDecayFactor;
                        if (!decayGreen)
                            maxColors.Y *= LightDecayThroughWater.Y * waterDecayFactor;
                        if (!decayBlue)
                            maxColors.Z *= LightDecayThroughWater.Z * waterDecayFactor;
                        break;

                    case LightMaskMode.Honey:
                        if (!decayRed)
                            maxColors.X *= LightDecayThroughHoney.X;
                        if (!decayGreen)
                            maxColors.Y *= LightDecayThroughHoney.Y;
                        if (!decayBlue)
                            maxColors.Z *= LightDecayThroughHoney.Z;
                        break;

                    default:
                        continue;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IndexOf(int x, int y) {
        return x * Height + y;
    }

    public void SetSize(int width, int height) {
        var length = (width + 1) * (height + 1);

        if (length > colors.Length) {
            colors = new Vector3[length];
            mask = new LightMaskMode[length];
        }

        Width = width;
        Height = height;
    }
}
