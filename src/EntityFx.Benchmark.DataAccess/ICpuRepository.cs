using System.Collections.Generic;
using System.Threading.Tasks;
using EntityFx.BenchmarkDb.Contracts;
using EntityFx.BenchmarkDb.Contracts.Cpu;

namespace EntityFx.BenchmarkDb.DataAccess
{
    public interface ICpuRepository
    {
        void Create(Cpu cpu);

        void Delete(int id);

        Task<IEnumerable<Cpu>> ReadAsync(CpuFilter page);
        Task<Cpu> ReadByIdAsync(int cpuId);
    }
}