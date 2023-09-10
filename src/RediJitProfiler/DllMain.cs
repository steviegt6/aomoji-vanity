using System;
using System.Runtime.InteropServices;
using ProfilerLib;

namespace RediJitProfiler;

public static partial class DllMain {
    private static ClassFactory instance = null!;

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(void* rclsid, void* riid, nint* ppv) {
        instance = new ClassFactory(new ReJitProfiler());
        *ppv = instance.IClassFactory;
        MessageBoxW(0, "DllGetClassObject", "DllMain", 0);
        Console.WriteLine($"rclsid: {(nint)rclsid}, riid: {(nint)riid}, ppv: {(nint)ppv}");
        return 0;
    }

    // import messagebox
    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int MessageBoxW(nint hWnd, string text, string caption, uint type);
}
