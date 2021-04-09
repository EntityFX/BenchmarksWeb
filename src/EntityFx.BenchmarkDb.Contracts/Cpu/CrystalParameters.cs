namespace EntityFx.BenchmarkDb.Contracts.Cpu
{
    public class CrystalParameters : PhysicalParameters
    {
        public decimal? ProcessInNm { get; set; }

        public ulong? TransistorsCount { get; set; }

        public uint? TDP { get; set; }


    }
}