using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Hikari.Lighting; 

public sealed class HikariLightingEngine : ILightingEngine {
    private sealed class LightingEngineRegistrationSystem : ModSystem {}

    public void Rebuild() {
        throw new System.NotImplementedException();
    }

    public void AddLight(int x, int y, Vector3 color) {
        throw new System.NotImplementedException();
    }

    public void ProcessArea(Rectangle area) {
        throw new System.NotImplementedException();
    }

    public Vector3 GetColor(int x, int y) {
        throw new System.NotImplementedException();
    }

    public void Clear() {
        throw new System.NotImplementedException();
    }
}
