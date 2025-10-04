using System.Linq.Expressions;

namespace TaskManager.Reports.Core.Typed;

public sealed class TypedReportsConfig<TProject, TTask>
{
    // BỎ required để tránh CS9035, ta sẽ validate khi đăng ký DI
    public Expression<Func<TProject, Guid>>? ProjectKey { get; set; }
    public Expression<Func<TProject, string>>? ProjectName { get; set; }
    public Expression<Func<TTask, Guid>>? TaskProjectKey { get; set; }

    // 3 cách nhận diện "Completed"
    private Func<TTask, bool>? _completedPredicate;
    private Func<string?, bool>? _completedByStatus;

    public void IsCompletedByPredicate(Expression<Func<TTask, bool>> pred)
        => _completedPredicate = pred.Compile();

    public void IsCompletedByStatus(Func<string?, bool> isCompleted)
        => _completedByStatus = isCompleted;

    public void IsCompletedByEnum<TEnum>(Func<TTask, TEnum> selector, TEnum completedValue)
        where TEnum : struct, Enum
        => _completedPredicate = t => EqualityComparer<TEnum>.Default.Equals(selector(t), completedValue);

    public bool IsCompleted(TTask t)
    {
        if (_completedPredicate != null) return _completedPredicate(t);

        if (_completedByStatus != null)
        {
            // Thử đọc thuộc tính "Status" (string?) bằng reflection nếu dùng IsCompletedByStatus
            var prop = typeof(TTask).GetProperty("Status");
            var value = prop?.GetValue(t)?.ToString();
            return _completedByStatus(value);
        }

        // Mặc định: không completed
        return false;
    }

    // Helpers để lấy delegate đã compile, có kiểm tra null, quăng lỗi cấu hình sớm
    public Func<TProject, Guid> GetProjectKey()
        => (ProjectKey ?? throw new InvalidOperationException("ReportsConfig: ProjectKey chưa được cấu hình.")).Compile();

    public Func<TProject, string> GetProjectName()
        => (ProjectName ?? throw new InvalidOperationException("ReportsConfig: ProjectName chưa được cấu hình.")).Compile();

    public Func<TTask, Guid> GetTaskProjectKey()
        => (TaskProjectKey ?? throw new InvalidOperationException("ReportsConfig: TaskProjectKey chưa được cấu hình.")).Compile();
}
