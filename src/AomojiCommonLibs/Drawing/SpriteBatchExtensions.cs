using AomojiCommonLibs.Reflection.RuntimeAccessor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AomojiCommonLibs.Drawing;

public static class SpriteBatchExtensions {
    public interface ISpriteBatchSnapshotAccessor {
        [FieldAccessor("sortMode")]
        SpriteSortMode SortMode { get; set; }

        [FieldAccessor("blendState")]
        BlendState BlendState { get; set; }

        [FieldAccessor("samplerState")]
        SamplerState SamplerState { get; set; }

        [FieldAccessor("depthStencilState")]
        DepthStencilState DepthStencilState { get; set; }

        [FieldAccessor("rasterizerState")]
        RasterizerState RasterizerState { get; set; }

        [FieldAccessor("customEffect")]
        Effect CustomEffect { get; set; }

        [FieldAccessor("transformMatrix")]
        Matrix TransformMatrix { get; set; }
    }

    public interface ISpriteBatchEndSafelyAccessor {
        [FieldAccessor("beginCalled")]
        bool BeginCalled { get; set; }
    }

    public static SpriteBatchSnapshot Capture(this SpriteBatch sb) {
        var accessor = RuntimeAccessorGenerator.GenerateAccessor<SpriteBatch, ISpriteBatchSnapshotAccessor>(sb);
        return new SpriteBatchSnapshot(accessor.SortMode, accessor.BlendState, accessor.SamplerState, accessor.DepthStencilState, accessor.RasterizerState, accessor.CustomEffect, accessor.TransformMatrix);
    }

    public static SpriteBatchSnapshot CaptureAndEndSafely(this SpriteBatch sb, out bool beginCalled) {
        var capture = sb.Capture();
        beginCalled = sb.EndSafely();
        return capture;
    }

    public static bool EndSafely(this SpriteBatch sb) {
        var accessor = RuntimeAccessorGenerator.GenerateAccessor<SpriteBatch, ISpriteBatchEndSafelyAccessor>(sb);
        var beginCalled = accessor.BeginCalled;
        if (beginCalled)
            sb.End();
        return beginCalled;
    }

    public static void BeginWithSnapshot(this SpriteBatch sb, SpriteBatchSnapshot capture) {
        sb.Begin(capture.SortMode, capture.BlendState, capture.SamplerState, capture.DepthStencilState, capture.RasterizerState, capture.Effect, capture.TransformMatrix);
    }
}
