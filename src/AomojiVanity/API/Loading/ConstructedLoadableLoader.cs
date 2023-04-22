using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AomojiVanity.API.Loading;

internal class ConstructedLoadableLoader {
    private List<IConstructedLoadable> loadables = new();

    public void Load(Mod mod) {
        foreach (var type in mod.Code.GetTypes()) {
            if (type.IsInterface || type.IsAbstract)
                continue;

            if (!typeof(IConstructedLoadable).IsAssignableFrom(type))
                continue;

            var loadable = (IConstructedLoadable) Activator.CreateInstance(type)!;
            loadables.Add(loadable);
            loadable.Load(mod);
        }
    }

    public void Unload() {
        foreach (var loadable in loadables)
            loadable.Unload();

        loadables = null!;
    }
}
