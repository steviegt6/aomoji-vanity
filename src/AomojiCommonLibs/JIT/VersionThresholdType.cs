namespace AomojiCommonLibs.JIT;

/// <summary>
///     The version threshold type for version-aware JIT.
/// </summary>
public enum VersionThresholdType {
    /// <summary>
    ///     The given version is the exact cutoff.
    /// </summary>
    Strict,

    /// <summary>
    ///     Lenience is provided and the given version is treated with semantic
    ///     versioning rules, meaning that loading is allowed without bounds of
    ///     the major version.
    /// </summary>
    SemVer,

    /// <summary>
    ///     Loading is always permitted regardless of the version (this is used
    ///     for logging when JITing types without guaranteed version support).
    /// </summary>
    Ignore,
}
