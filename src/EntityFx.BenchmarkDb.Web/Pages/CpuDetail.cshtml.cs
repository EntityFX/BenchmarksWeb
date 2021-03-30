﻿using System.Threading.Tasks;
using EntityFx.BenchmarkDb.Contracts.Cpu;
using EntityFx.BenchmarkDb.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EntityFx.BenchmarkDb.Web.Pages
{
    public class CpuDetailModel : PageModel
    {
        private readonly ILogger<CpuDetailModel> _logger;

        private readonly ICpuRepository _cpuRepository;

        public Cpu CpuDetail;

        public CpuDetailModel(ILogger<CpuDetailModel> logger, ICpuRepository cpuRepository)
        {
            _logger = logger;
            _cpuRepository = cpuRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CpuDetail = await _cpuRepository.ReadByIdAsync(id);

            if (CpuDetail == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}