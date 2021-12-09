using System;

namespace EntityFx.BenchmarkDb.DataAccess
{
    class BenchmarkResultEntity
    {

        public int Id { get; set; }
        public int BenchmarkResultId { get; set; }

        public int CpuId { get; set; }

        public string FileName { get; set; }

        public string Benchmark { get; set; }

        public string Output { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public decimal Value { get; set; }

        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public decimal Value3 { get; set; }
        public decimal Value4 { get; set; }
        public decimal Value5 { get; set; }
        public decimal Value6 { get; set; }
        public decimal Value7 { get; set; }
        public decimal Value8 { get; set; }
        public decimal Value9 { get; set; }

        public string UnitsOfMeasure { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
