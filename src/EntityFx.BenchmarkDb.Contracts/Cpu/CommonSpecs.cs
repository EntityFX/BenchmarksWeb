using System.Collections.Generic;

namespace EntityFx.BenchmarkDb.Contracts.Cpu
{
    public class CommonSpecs
    {
        public string InstructionSet { get; set; }

        public string Features { get; set; }

        public string MicroArchitecture { get; set; }

        public uint? ClockInMhz { get; set; }

        public decimal? MaxClockInMhz { get; set; }

        public decimal? BusInMhz { get; set; }

        public decimal? Multiplier { get; set; }

        public uint Cores { get; set; }

        public uint Threads { get; set; }

        public IEnumerable<string> SupportedInstructions { get; set; }
    }
}
