namespace EntityFx.BenchmarkDb.Contracts.Cpu
{
    public class CacheItem
    {
        public uint? SizeKBytes { get; set; }

        public CacheAssociativity? CacheAssociativity { get; set; }

        public uint? LineSizeInBytes { get; set; }

        public bool? IsShared { get; set; }

        public string Details { get; set; }
    }
}