using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using ReLogic.Threading;
using Terraria.Graphics.Light;

namespace AomojiVanity.Benchmarks;

[MemoryDiagnoser]
public class ParallelBenchmarks {
    [Params(203 * 203)]
    public int Length;

    private Vector3[] colors = new Vector3[203 * 203];
    private LightMaskMode[] mask = new LightMaskMode[203 * 203];

    [Benchmark]
    public void UseParallelFor() {
        Parallel.For(
            0,
            Length,
            i => {
                colors[i] = Vector3.Zero;
                mask[i] = LightMaskMode.None;
            }
        );
    }

    [Benchmark]
    public void UseFastParallelFor() {
        FastParallel.For(
            0,
            Length,
            (start, end, _) => {
                for (var i = start; i < end; i++) {
                    colors[i] = Vector3.Zero;
                    mask[i] = LightMaskMode.None;
                }
            }
        );
    }
}
