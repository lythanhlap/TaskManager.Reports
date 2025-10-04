using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using TaskManager.Reports.Abstractions;

namespace TaskManager.Reports.Mvc.ViewComponents;

public sealed class ProjectProgressViewComponent : ViewComponent
{
    private readonly IReportReadOnly _reports;
    public ProjectProgressViewComponent(IReportReadOnly reports) => _reports = reports;

    public async Task<IViewComponentResult> InvokeAsync(Guid projectId, CancellationToken ct = default)
    {
        var vm = await _reports.GetProjectAsync(projectId, ct);
        return View("Default", vm);
    }
}
