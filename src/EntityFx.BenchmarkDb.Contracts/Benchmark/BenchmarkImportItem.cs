using System.Collections.Generic;

namespace EntityFx.BenchmarkDb.Contracts.Benchmark
{
    public class BenchmarkImportItem
    {
        public Dictionary<string, string> Details { get; set; }

        public Dictionary<string, decimal> Results { get; set; }

        public Cpu.Cpu CpuInfo { get; set; }
    }
}