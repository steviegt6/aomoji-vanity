using JetBrains.Annotations;
using Terraria.ModLoader;

namespace Hikari;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class Hikari : Mod {
    /*public override void Load() {
        base.Load();

        // REALLY EVIL: Uncap FPS and disable VSync.
        Main.instance.IsFixedTimeStep = false;
        Main.graphics.SynchronizeWithVerticalRetrace = false;
        Main.superFast = true;
    }*/
}
