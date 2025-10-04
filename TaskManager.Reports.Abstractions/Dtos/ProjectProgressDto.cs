using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Reports.Abstractions.Dtos;

public sealed class ProjectProgressDto
{
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = "";
    public int CompletedCount { get; init; }
    public int PendingCount { get; init; }
    public int TotalCount => CompletedCount + PendingCount;
    public double CompletedPercent => TotalCount == 0 ? 0 : (CompletedCount * 100.0 / TotalCount);
}
