using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Terraria;
using Terraria.ModLoader;

namespace RediJit;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public class RediJit : Mod {
    private const string dll_path_base = "RediJit.native.RediJitProfiler-{0}.dll";

    private static string DllPath => Path.Combine(Main.SavePath, "aomoji", "profiler.dll");

    private static string VersionPath => Path.Combine(Main.SavePath, "aomoji", "redijit_version.txt");

    public override void Load() {
        base.Load();

        Directory.CreateDirectory(Path.Combine(Main.SavePath, "aomoji"));

        var justInstalledProfiler = false;

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

        Logger.Debug("Just installed profiler: " + justInstalledProfiler);

        if (justInstalledProfiler || !IsProfilerPresent()) {
            Logger.Debug("Injecting profiler into game startup...");
            InjectProfilerIntoStartupFiles();

            Logger.Debug("Restarting game...");
            RestartGame();
        }

        /*var handle = NativeLibrary.Load(DllPath);
        Logger.Debug($"Loaded DLL at {DllPath} into memory at 0x{handle:X}");*/
    }

    private void InstallProfiler() {
        if (File.Exists(DllPath))
            File.Delete(DllPath);

        if (File.Exists(VersionPath))
            File.Delete(VersionPath);

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

        File.WriteAllText(VersionPath, Version.ToString());
    }

    private void InjectProfilerIntoStartupFiles() {
        var workingDir = Directory.GetCurrentDirectory();
    }

    private void RestartGame() {
        var scriptExt = OperatingSystem.IsWindows() ? ".bat" : ".sh";
        var scriptName = Main.dedServ ? "start-tModLoaderServer" : "start-tModLoader";
        if (Environment.GetEnvironmentVariable("SteamClientLaunch") == "1")
            scriptName += "-FamilyShare";
        scriptName += scriptExt;

        if (OperatingSystem.IsWindows()) {
            Process.Start(new ProcessStartInfo {
                FileName = "cmd.exe",
                Arguments = $"/c \"{scriptName}\"",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = true,
            });
        }
        else {
            Process.Start(new ProcessStartInfo {
                FileName = $"./{scriptName}",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = true,
            });
        }

        Logger.Debug("Exiting...");
        Environment.Exit(0);
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

    private static bool IsProfilerPresent() {
        var present = true;
        present &= Environment.GetEnvironmentVariable("CORECLR_ENABLE_PROFILING") == "1";
        present &= Environment.GetEnvironmentVariable("CORECLR_PROFILER") == "{846F5F1C-F9AE-4B07-969E-05C26BC060D8}";
        present &= Environment.GetEnvironmentVariable("CORECLR_PROFILER_PATH") == DllPath; // Or check if it's not null?
        return present;
    }
}
