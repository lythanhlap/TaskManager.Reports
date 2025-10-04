using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskManager.Reports.Abstractions.Dtos;

namespace TaskManager.Reports.Abstractions;

public interface IReportReadOnly
{
    Task<PortfolioSummaryDto> GetPortfolioAsync(CancellationToken ct);
    Task<ProjectProgressDto> GetProjectAsync(Guid projectId, CancellationToken ct);
}
