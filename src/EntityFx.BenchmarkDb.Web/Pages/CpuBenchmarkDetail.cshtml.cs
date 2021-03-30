using System.Threading.Tasks;
using EntityFx.BenchmarkDb.Contracts.Benchmark;
using EntityFx.BenchmarkDb.Contracts.Cpu;
using EntityFx.BenchmarkDb.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EntityFx.BenchmarkDb.Web.Pages
{
    public class CpuBenchmarkDetailModel : PageModel
    {
        private readonly ILogger<CpuBenchmarkDetailModel> _logger;

        private readonly ICpuRepository _cpuRepository;
        private readonly IBenchmarkResultsRepository _benchmarkResultsRepository;

        public BenchmarkResult BenchmarkResult;

        public Cpu Cpu;

        [BindProperty(SupportsGet = true)]
        public int? CpuId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int BenchmarkId { get; set; }

        public CpuBenchmarkDetailModel(ILogger<CpuBenchmarkDetailModel> logger, ICpuRepository cpuRepository,
            IBenchmarkResultsRepository benchmarkResultsRepository)
        {
            _logger = logger;
            _cpuRepository = cpuRepository;
            _benchmarkResultsRepository = benchmarkResultsRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            BenchmarkResult = await _benchmarkResultsRepository.ReadByIdAsync(BenchmarkId);

            if (BenchmarkResult == null)
            {
                return NotFound();
            }

            if (CpuId == null)
            {
                CpuId = BenchmarkResult.CpuId;
            }

            Cpu = await _cpuRepository.ReadByIdAsync(CpuId.Value);

            return Page();
        }
    }
}