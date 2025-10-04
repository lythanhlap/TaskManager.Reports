using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Reports.Abstractions;
using TaskManager.Reports.Core.Sources;

namespace TaskManager.Reports.Core.Typed;

public static class ServiceCollectionExtensionsTwoDb
{
    /// <summary>
    /// Đăng ký Report đọc từ HAI DbContext khác nhau:
    /// - TProjDb: DbContext chứa Project
    /// - TTaskDb: DbContext chứa Task
    /// </summary>
    public static IServiceCollection AddReportsCoreTwoDb<TProjDb, TTaskDb, TProject, TTask>(
        this IServiceCollection services,
        Action<TypedReportsConfig<TProject, TTask>> configure)
        where TProjDb : Microsoft.EntityFrameworkCore.DbContext
        where TTaskDb : Microsoft.EntityFrameworkCore.DbContext
        where TProject : class
        where TTask : class
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        var cfg = new TypedReportsConfig<TProject, TTask>();
        configure(cfg);

        // Validate 3 mapping bắt buộc
        _ = cfg.ProjectKey     ?? throw new InvalidOperationException("ReportsConfig: ProjectKey chưa cấu hình.");
        _ = cfg.ProjectName    ?? throw new InvalidOperationException("ReportsConfig: ProjectName chưa cấu hình.");
        _ = cfg.TaskProjectKey ?? throw new InvalidOperationException("ReportsConfig: TaskProjectKey chưa cấu hình.");

        services.AddSingleton(cfg);
        services.AddScoped<ITaskProgressSource, TypedEfTaskProgressSourceTwoDb<TProjDb, TTaskDb, TProject, TTask>>();
        services.AddScoped<IReportReadOnly, ReportService>();
        return services;
    }
}
