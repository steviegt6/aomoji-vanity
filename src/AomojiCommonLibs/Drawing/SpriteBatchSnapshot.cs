using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AomojiCommonLibs.Drawing;

public struct SpriteBatchSnapshot {
    public SpriteSortMode SortMode { get; set; }

    public BlendState BlendState { get; set; }

    public SamplerState SamplerState { get; set; }

    public DepthStencilState DepthStencilState { get; set; }

    public RasterizerState RasterizerState { get; set; }

    public Effect Effect { get; set; }

    public Matrix TransformMatrix { get; set; }

    public SpriteBatchSnapshot(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix) {
        SortMode = sortMode;
        BlendState = blendState;
        SamplerState = samplerState;
        DepthStencilState = depthStencilState;
        RasterizerState = rasterizerState;
        Effect = effect;
        TransformMatrix = transformMatrix;
    }
}
