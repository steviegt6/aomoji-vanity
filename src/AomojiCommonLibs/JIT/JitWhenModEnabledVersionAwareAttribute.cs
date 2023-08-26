using System;
using System.Reflection;
using Terraria.ModLoader;

namespace AomojiCommonLibs.JIT;

public class JitWhenModEnabledVersionAwareAttribute : MemberJitAttribute {
    public string ModName { get; }

    public string MinimumVersion { get; }

    public string MaximumVersion { get; }

    public VersionThresholdType MinimumVersionThreshold { get; }

    public VersionThresholdType MaximumVersionThreshold { get; }

    public JitWhenModEnabledVersionAwareAttribute(
        string modName,
        string minimumVersion,
        string maximumVersion,
        VersionThresholdType minimumVersionThreshold = VersionThresholdType.Strict,
        VersionThresholdType maximumVersionThreshold = VersionThresholdType.Strict
    ) {
        ModName = modName;
        MinimumVersion = minimumVersion;
        MaximumVersion = maximumVersion;
        MinimumVersionThreshold = minimumVersionThreshold;
        MaximumVersionThreshold = maximumVersionThreshold;
    }

    public override bool ShouldJIT(MemberInfo member) {
        if (!ModLoader.TryGetMod(ModName, out var mod))
            return false;

        var modVersion = CreateStandardVersion(mod.Version);
        var minimumVersion = CreateStandardVersion(new Version(MinimumVersion));
        var maximumVersion = CreateStandardVersion(new Version(MaximumVersion));

        var minimumIgnored = false;
        var meetsMinimum = MinimumVersionThreshold switch {
            VersionThresholdType.Strict => modVersion >= minimumVersion,
            VersionThresholdType.SemVer => modVersion.Major >= minimumVersion.Major,
            VersionThresholdType.Ignore => minimumIgnored = true,
            _ => throw new InvalidOperationException("Invalid minimum version threshold type."),
        };

        var maximumIgnored = false;
        var meetsMaximum = MaximumVersionThreshold switch {
            VersionThresholdType.Strict => modVersion <= maximumVersion,
            VersionThresholdType.SemVer => modVersion.Major <= maximumVersion.Major,
            VersionThresholdType.Ignore => maximumIgnored = true,
            _ => throw new InvalidOperationException("Invalid maximum version threshold type."),
        };

        if (minimumIgnored || maximumIgnored)
            ModContent.GetInstance<AomojiCommonLibs>().RegisterIgnoredJitWarning(ModName, MinimumVersion, MaximumVersion, meetsMinimum, meetsMaximum, member.Name);

        return meetsMinimum && meetsMaximum;
    }

    private static Version CreateStandardVersion(Version version) {
        return new Version(Math.Max(version.Major, 0), Math.Max(version.Minor, 0), Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
    }
}
