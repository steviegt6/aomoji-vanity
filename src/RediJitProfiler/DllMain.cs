using System;
using System.Runtime.InteropServices;
using ProfilerLib;

namespace RediJitProfiler;

public static class DllMain {
    private static ClassFactory instance = null!;

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(void* rclsid, void* riid, nint* ppv) {
        instance = new ClassFactory(new ReJitProfiler());
        *ppv = instance.IClassFactory;
        Console.WriteLine($"rclsid: {(nint)rclsid}, riid: {(nint)riid}, ppv: {(nint)ppv}");
        return 0;
    }
}
