using System;
using Microsoft.Xna.Framework.Graphics;

namespace AomojiCommonLibs.Drawing;

/// <summary>
///     A disposable reference to a
///     <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> which handles
///     ending and beginning/restarting the batch
/// </summary>
public sealed class DisposableSpritebatch : IDisposable {
    public SpriteBatch SpriteBatch { get; }

    private readonly SpriteBatchSnapshot originalSnapshot;
    private readonly bool wasBegun;

    public DisposableSpritebatch(SpriteBatch spriteBatch, SpriteBatchSnapshot snapshot) {
        SpriteBatch = spriteBatch;
        originalSnapshot = SpriteBatch.CaptureAndEndSafely(out wasBegun);
        spriteBatch.BeginWithSnapshot(snapshot);
    }

    public void Dispose() {
        SpriteBatch.EndSafely();
        if (wasBegun)
            SpriteBatch.BeginWithSnapshot(originalSnapshot);
    }
}
