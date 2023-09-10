using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Microsoft.Diagnostics.NETCore.Client;
using Terraria;
using Terraria.ModLoader;

namespace RediJit;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class RediJit : Mod {
    private const string dll_path_base = "RediJit.native.RediJitProfiler-{0}.dll";

    private static string DllPath => Path.Combine(Main.SavePath, "aomoji", "profiler.dll");

    // private static string VersionPath => Path.Combine(Main.SavePath, "aomoji", "redijit_version.txt");

    public override void Load() {
        base.Load();

        Directory.CreateDirectory(Path.Combine(Main.SavePath, "aomoji"));

        /*var justInstalledProfiler = false;

        if (!File.Exists(VersionPath) || !File.Exists(DllPath)) {
            InstallProfiler();
            justInstalledProfiler = true;
        }
        else {
            var version = File.ReadAllText(VersionPath);

            if (new Version(version) < Version) {
                InstallProfiler();
                justInstalledProfiler = true;
            }
        }

        Logger.Debug("Just installed profiler: " + justInstalledProfiler);*/

        InstallProfiler();

        Main.QueueMainThreadAction(() => {
            var client = new DiagnosticsClient(Environment.ProcessId);
            client.AttachProfiler(TimeSpan.FromSeconds(10), new Guid("cf0d821e-299b-5307-a3d8-b283c03916dd"), DllPath);
        });

        /*var handle = NativeLibrary.Load(DllPath);
        Logger.Debug($"Loaded DLL at {DllPath} into memory at 0x{handle:X}");*/
    }

    private void InstallProfiler() {
        if (File.Exists(DllPath))
            File.Delete(DllPath);

        /*if (File.Exists(VersionPath))
            File.Delete(VersionPath);*/

        var rid = GetRid();
        var dllAsmPath = string.Format(dll_path_base, rid);
        Logger.Debug($"Using RID: {rid}");
        Logger.Debug($"Using internal DLL path: {dllAsmPath}");
        var asm = typeof(RediJit).Assembly;
        var dllStream = asm.GetManifestResourceStream(dllAsmPath);
        if (dllStream is null)
            throw new PlatformNotSupportedException($"Could not find embedded DLL at path: {dllAsmPath}; your RID is not supported!");

        var fs = new FileStream(DllPath, FileMode.Create, FileAccess.Write);
        dllStream.CopyTo(fs);
        Logger.Debug($"Wrote embedded DLL to {DllPath}");
        fs.Dispose();

        // File.WriteAllText(VersionPath, Version.ToString());
    }

    private static string GetRid() {
        string os;

        if (OperatingSystem.IsWindows())
            os = "win";
        else if (OperatingSystem.IsLinux())
            os = "linux";
        else if (OperatingSystem.IsMacOS())
            os = "mac";
        else
            throw new NotSupportedException("Unsupported operating system.");

        var arch = Environment.Is64BitProcess ? "x64" : "x86";
        return $"{os}-{arch}";
    }
}
