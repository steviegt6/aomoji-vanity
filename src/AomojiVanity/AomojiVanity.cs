using AomojiVanity.API.Loading;
using JetBrains.Annotations;
using Terraria.ModLoader;

namespace AomojiVanity;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class AomojiVanity : Mod {
    private ConstructedLoadableLoader constructedLoadableLoader;

    public AomojiVanity() {
        constructedLoadableLoader = new ConstructedLoadableLoader();
        constructedLoadableLoader.Load(this);
    }

    public override void Unload() {
        base.Unload();

        constructedLoadableLoader.Unload();
        constructedLoadableLoader = null!;
    }
}
