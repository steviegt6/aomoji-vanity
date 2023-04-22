namespace AomojiVanity.API.ModHijack;

internal record HijackResult(bool Hijacked, object? Result) {
    public static readonly HijackResult NOT_HIJACKED = new(false, null);
}
