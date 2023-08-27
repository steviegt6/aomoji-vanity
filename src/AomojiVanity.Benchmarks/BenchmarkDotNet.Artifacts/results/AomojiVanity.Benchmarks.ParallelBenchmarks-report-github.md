``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2965/22H2/2022Update)
AMD Ryzen 7 5700G with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2


```
|             Method | Length |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|------------------- |------- |---------:|---------:|---------:|-------:|----------:|
|     UseParallelFor |  41209 | 41.31 μs | 0.995 μs | 2.856 μs | 0.5493 |    4.6 KB |
| UseFastParallelFor |  41209 | 19.73 μs | 0.391 μs | 0.366 μs | 0.2441 |   2.16 KB |
