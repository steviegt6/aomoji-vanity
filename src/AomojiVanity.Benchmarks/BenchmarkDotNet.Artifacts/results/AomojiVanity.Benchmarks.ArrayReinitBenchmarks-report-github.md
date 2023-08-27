``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2


```
|                                 Method | Length |     Mean |    Error |   StdDev |   Median |     Gen0 |     Gen1 |     Gen2 | Allocated |
|--------------------------------------- |------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|----------:|
|                         UseParallelFor |  41209 | 40.93 μs | 0.921 μs | 2.672 μs | 40.60 μs |   0.5493 |        - |        - |    4742 B |
|                     UseFastParallelFor |  41209 | 20.86 μs | 0.410 μs | 0.472 μs | 20.65 μs |   0.2441 |        - |        - |    2208 B |
|                                 UseFor |  41209 | 74.08 μs | 1.461 μs | 2.919 μs | 75.31 μs |        - |        - |        - |         - |
|                              UseReinit |  41209 | 26.31 μs | 0.484 μs | 1.206 μs | 26.25 μs | 142.8528 | 142.8528 | 142.8528 |  535824 B |
| UseFastParallelForWithInnerParallelFor |  41209 | 51.36 μs | 0.261 μs | 0.244 μs | 51.29 μs |   5.3101 |   0.1221 |        - |   43796 B |
