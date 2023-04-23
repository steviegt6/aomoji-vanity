using AomojiVanity.API.Hijacking;

namespace AomojiVanity.API;

internal static class Unloader {
    public static void Unload() {
        ModHijackLoader.Unload();
    }
}
