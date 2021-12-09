using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using EntityFx.BenchmarkDb.Contracts;
using EntityFx.BenchmarkDb.Contracts.Benchmark;
using EntityFx.BenchmarkDb.Contracts.Cpu;
using EntityFx.BenchmarkDb.DataAccess;
using NUnit.Framework;

namespace EntityFx.Tests
{
    public class CpuRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            SqliteRepositoryBase.FileNameInternal = "benchmarks.sqlite3";

            var cpuRepository = new CpuRepository();

            var cpu = new Cpu()
            {
                Manufacturer = "Intel",
                Model = "Core i7 2600",
                Category = "x86"
            };

            cpuRepository.Create(cpu);
        }

        [Test]
        public async Task TestRead()
        {
            SqliteRepositoryBase.FileNameInternal = "benchmarks.sqlite3";

            var cpuRepository = new CpuRepository();

            var cpus = (await cpuRepository.ReadAsync(new CpuFilter())).ToArray();
        }

        [Test]
        public async Task LoadResults()
        {
            var results = File.ReadAllText("AllResults.json");
            var benchmarks = JsonSerializer.Deserialize<Dictionary<string, BenchmarkImportItem>>(results);

            SqliteRepositoryBase.FileNameInternal = "benchmarks.sqlite3";

            var benchmarkResultsRepository = new BenchmarkResultsRepository();
            var cpuRepository = new CpuRepository();
            foreach (var cpuBenchmarks in benchmarks)
            {
                var cpu = (await cpuRepository.ReadAsync(new CpuFilter() { SearchString = cpuBenchmarks.Key })).FirstOrDefault();

                if (cpu == null)
                {
                    continue;
                }

                foreach (var cpuBenchmark in cpuBenchmarks.Value.Details)
                {
                    var benchmarkResult = new BenchmarkResult()
                    {
                        Value = cpuBenchmarks.Value.Results[cpuBenchmark.Key],
                        CpuId = cpu.Id,
                        Output = cpuBenchmark.Value,
                        Category = "Native",
                        SubCategory = cpuBenchmark.Key,
                        Benchmark = cpuBenchmark.Key,
                    };
                    benchmarkResultsRepository.Create(benchmarkResult);
                }
            }


        }


        [Test]
        public void LoadCpus()
        {
            var cpusList = ReadCpusCsv("Cpus.csv");

            SqliteRepositoryBase.FileNameInternal = "benchmarks.sqlite3";

            var cpuRepository = new CpuRepository();

            foreach (var cpuItem in cpusList)
            {
                var cpu = new Cpu()
                {
                    Name = cpuItem["Name"],
                    Manufacturer = cpuItem["Vendor"],
                    Model = cpuItem["Model"],
                    Category = cpuItem["Architecture"],
                    Specs = new CommonSpecs()
                    {
                        Cores = 1,
                        Threads = 1
                    }
                };

                cpuRepository.Merge(cpu);
            }
        }

        private IEnumerable<IDictionary<string, string>> ReadCpusCsv(string filePath)
        {
            var cpus = File.ReadAllLines(filePath);

            var headers = cpus.First().Split(',').ToArray();

            return cpus.Skip(1).Select(s => s.Split(',')
                .Select((it, i) => new KeyValuePair<string, string>(headers[i], it))
                .ToDictionary(kv => kv.Key, kv => kv.Value));
        }
    }
}