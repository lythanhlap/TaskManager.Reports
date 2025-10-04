using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TaskManager.Reports.Core.Sources;

public interface ITaskProgressSource
{
    Task<IReadOnlyList<(Guid ProjectId, string ProjectName, int Completed, int Pending)>>
        GetAllProjectTaskCountsAsync(CancellationToken ct);

    Task<(string ProjectName, int Completed, int Pending)>
        GetProjectTaskCountsAsync(Guid projectId, CancellationToken ct);
}