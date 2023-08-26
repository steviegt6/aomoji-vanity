using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace Hikari.Content.Lighting;

public sealed class HikariLightingEngine : ILightingEngine {
    public void Rebuild() { }

    public void AddLight(int x, int y, Vector3 color) { }

    public void ProcessArea(Rectangle area) { }

    public Vector3 GetColor(int x, int y) {
        return Vector3.One;
    }

    public void Clear() { }
}
