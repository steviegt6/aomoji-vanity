using ProfilerLib;

namespace RediJitProfiler;

public sealed class ReJitProfiler : CorProfilerCallback10Base {
    protected override HResult Initialize(int iCorProfilerInfoVersion) {
        // if (iCorProfilerInfoVersion < 11)
        //    return HResult.E_FAIL;

        // Minimum 10 because of the new re-JIT on attach API:
        // https://github.com/dotnet/runtime/blob/main/docs/design/coreclr/profiling/ReJIT%20on%20Attach.md
        if (iCorProfilerInfoVersion < 10)
            return HResult.E_FAIL;

        // TODO: Determine the bare minimum we need to monitor.
        return ICorProfilerInfo5.SetEventMask2(CorPrfMonitor.COR_PRF_MONITOR_ALL, CorPrfHighMonitor.COR_PRF_HIGH_MONITOR_DYNAMIC_FUNCTION_UNLOADS);
    }
}
