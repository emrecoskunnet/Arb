using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ArbTech.Infrastructure.BackgroundJobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class JobWrapper<TAppJob>(IServiceProvider provider) : IJob
    where TAppJob: IAppJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var job = provider.GetRequiredService<TAppJob>();
        await job.Execute(context.MergedJobDataMap, context.CancellationToken);
    }
}
