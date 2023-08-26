using System.Collections.Generic;
using JetBrains.Annotations;
using Terraria.ModLoader;

namespace AomojiCommonLibs;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class AomojiCommonLibs : Mod {
    private readonly struct IgnoredJitWarning {
        public string ModName { get; }

        public string MinimumVersion { get; }

        public string MaximumVersion { get; }

        public bool MetMinimum { get; }

        public bool MetMaximum { get; }

        public string MemberInfoName { get; }

        public IgnoredJitWarning(string modName, string minimumVersion, string maximumVersion, bool metMinimum, bool metMaximum, string memberInfoName) {
            ModName = modName;
            MinimumVersion = minimumVersion;
            MaximumVersion = maximumVersion;
            MetMinimum = metMinimum;
            MetMaximum = metMaximum;
            MemberInfoName = memberInfoName;
        }
    }

    private readonly List<IgnoredJitWarning> ignoredJitWarnings = new();

    public override void PostSetupContent() {
        base.PostSetupContent();

        foreach (var warning in ignoredJitWarnings)
            Logger.Warn($"Ignored JIT requirements for member '{warning.MemberInfoName}: {warning.ModName} {warning.MinimumVersion} (met: {warning.MetMinimum}) {warning.MaximumVersion} (met: {warning.MetMaximum})'.");
    }

    internal void RegisterIgnoredJitWarning(string modName, string minimumVersion, string maximumVersion, bool metMinimum, bool metMaximum, string memberInfoName) {
        ignoredJitWarnings.Add(new IgnoredJitWarning(modName, minimumVersion, maximumVersion, metMinimum, metMaximum, memberInfoName));
    }
}
