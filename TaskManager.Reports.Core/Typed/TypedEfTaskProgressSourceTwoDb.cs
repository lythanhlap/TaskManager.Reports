using Microsoft.EntityFrameworkCore;
using TaskManager.Reports.Core.Sources;

namespace TaskManager.Reports.Core.Typed;

/// <summary>
/// Nguồn dữ liệu Report đọc từ HAI DbContext tách biệt:
/// - _projDb: chứa Projects
/// - _taskDb: chứa Tasks
/// </summary>
public sealed class TypedEfTaskProgressSourceTwoDb<TProjDb, TTaskDb, TProject, TTask> : ITaskProgressSource
    where TProjDb : DbContext
    where TTaskDb : DbContext
    where TProject : class
    where TTask : class
{
    private readonly TProjDb _projDb;
    private readonly TTaskDb _taskDb;
    private readonly TypedReportsConfig<TProject, TTask> _cfg;

    public TypedEfTaskProgressSourceTwoDb(
        TProjDb projDb,
        TTaskDb taskDb,
        TypedReportsConfig<TProject, TTask> cfg)
    {
        _projDb = projDb;
        _taskDb = taskDb;
        _cfg = cfg;
    }

    public async Task<IReadOnlyList<(Guid ProjectId, string ProjectName, int Completed, int Pending)>>
        GetAllProjectTaskCountsAsync(CancellationToken ct)
    {
        var projects = await _projDb.Set<TProject>().AsNoTracking().ToListAsync(ct);
        var tasks = await _taskDb.Set<TTask>().AsNoTracking().ToListAsync(ct);

        var pKey = _cfg.GetProjectKey();
        var pName = _cfg.GetProjectName();
        var tPkey = _cfg.GetTaskProjectKey();

        // group task theo ProjectId
        var groups = tasks.GroupBy(t => tPkey(t));

        // map ProjectId -> (Completed, Pending)
        var map = new Dictionary<Guid, (int Completed, int Pending)>();
        foreach (var g in groups)
        {
            int completed = 0;
            foreach (var t in g)
                if (_cfg.IsCompleted(t)) completed++;
            int pending = g.Count() - completed;
            map[g.Key] = (completed, pending);
        }

        // trả về đầy đủ cho tất cả project (kể cả không có task)
        var list = new List<(Guid, string, int, int)>(projects.Count);
        foreach (var p in projects)
        {
            var id = pKey(p);
            var name = pName(p);
            var (completed, pending) = map.TryGetValue(id, out var agg) ? agg : (0, 0);
            list.Add((id, name, completed, pending));
        }
        return list;
    }

    public async Task<(string ProjectName, int Completed, int Pending)>
        GetProjectTaskCountsAsync(Guid projectId, CancellationToken ct)
    {
        var pKey = _cfg.GetProjectKey();
        var pName = _cfg.GetProjectName();
        var tPkey = _cfg.GetTaskProjectKey();

        var projectName = await _projDb.Set<TProject>()
            .AsNoTracking()
            .Select(x => new { Id = pKey(x), Name = pName(x) })
            .Where(x => x.Id == projectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(ct) ?? string.Empty;

        var tlist = await _taskDb.Set<TTask>()
            .AsNoTracking()
            .Where(x => tPkey(x) == projectId)
            .ToListAsync(ct);

        int completed = 0;
        foreach (var t in tlist)
            if (_cfg.IsCompleted(t)) completed++;
        int pending = tlist.Count - completed;

        return (projectName, completed, pending);
    }
}
