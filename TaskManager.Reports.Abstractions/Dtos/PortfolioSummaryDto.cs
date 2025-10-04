using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Reports.Abstractions.Dtos;
public sealed class PortfolioSummaryDto
{
    public int TotalProjects { get; init; }
    public int TotalTasks { get; init; }
    public int TotalCompleted { get; init; }
    public int TotalPending { get; init; }
    public double CompletedPercent => TotalTasks == 0 ? 0 : (TotalCompleted * 100.0 / TotalTasks);
    public IReadOnlyList<ProjectProgressDto> Items { get; init; } = Array.Empty<ProjectProgressDto>();
}