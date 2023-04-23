using AomojiVanity.API;
using JetBrains.Annotations;
using Terraria.ModLoader;

namespace AomojiVanity;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class AomojiVanity : Mod {
    public override void Unload() {
        base.Unload();

        Unloader.Unload();
    }
}
