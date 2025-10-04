using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Reports.Abstractions;


namespace TaskManager.Reports.Mvc.Controllers
{
    [Authorize]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/reports")] 
    public sealed class ReportsApiController : ControllerBase
    {
        private readonly IReportReadOnly _reports;
        public ReportsApiController(IReportReadOnly reports) => _reports = reports;

        [HttpGet("portfolio")]
        public async Task<IActionResult> Portfolio(CancellationToken ct)
            => Ok(await _reports.GetPortfolioAsync(ct));

        [HttpGet("projects/{projectId:guid}")]
        public async Task<IActionResult> Project(Guid projectId, CancellationToken ct)
            => Ok(await _reports.GetProjectAsync(projectId, ct));
    }
}
