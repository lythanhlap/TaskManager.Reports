using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Reports.Abstractions;
using TaskManager.Reports.Abstractions.Dtos;
using TaskManager.Reports.Core.Sources;

namespace TaskManager.Reports.Core;

public sealed class ReportService : IReportReadOnly
{
    private readonly ITaskProgressSource _src;
    public ReportService(ITaskProgressSource src) => _src = src;

    public async Task<PortfolioSummaryDto> GetPortfolioAsync(CancellationToken ct)
    {
        var rows = await _src.GetAllProjectTaskCountsAsync(ct);
        var items = rows.Select(x => new ProjectProgressDto
        {
            ProjectId = x.ProjectId,
            ProjectName = x.ProjectName,
            CompletedCount = x.Completed,
            PendingCount = x.Pending
        }).ToList();

        var totalCompleted = items.Sum(i => i.CompletedCount);
        var totalPending = items.Sum(i => i.PendingCount);
        return new PortfolioSummaryDto
        {
            TotalProjects = items.Count,
            TotalTasks = totalCompleted + totalPending,
            TotalCompleted = totalCompleted,
            TotalPending = totalPending,
            Items = items
        };
    }

    public async Task<ProjectProgressDto> GetProjectAsync(Guid projectId, CancellationToken ct)
    {
        var (name, completed, pending) = await _src.GetProjectTaskCountsAsync(projectId, ct);
        return new ProjectProgressDto
        {
            ProjectId = projectId,
            ProjectName = name,
            CompletedCount = completed,
            PendingCount = pending
        };
    }
}
