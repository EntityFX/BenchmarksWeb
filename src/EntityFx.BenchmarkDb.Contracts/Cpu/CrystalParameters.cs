namespace EntityFx.BenchmarkDb.Contracts.Cpu
{
    public class CrystalParameters : PhysicalParameters
    {
        public decimal? ProcessInNm { get; set; }

        public uint? TransistorsCount { get; set; }

        public uint? TDP { get; set; }


    }
}