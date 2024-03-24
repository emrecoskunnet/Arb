using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using ArbTech.Infrastructure.Configuration;

namespace ArbTech.Infrastructure.BackgroundJobs;

public static class QuartzExtensions
{
    public static void AddCustomBackgroundJobWithQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        // base configuration from appsettings.json
        services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

        // if you are using persistent job store, you might want to alter some options
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        
        services.AddQuartz(q =>
        {

            string clientId = configuration.GetRequiredMicroserviceMetaData().ClientId;
            
            q.SchedulerId = $"Scheduler-{clientId}"; 
 
            q.UseSimpleTypeLoader();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
             
            q.UseTimeZoneConverter();
 
            q.UseJobAutoInterrupt(options =>
            { 
                options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
            });
            
            q.UseInMemoryStore();
            //
            // q.UsePersistentStore(s =>
            // {
            //     s.PerformSchemaValidation = true; 
            //     s.UseProperties = true; 
            //     s.RetryInterval = TimeSpan.FromSeconds(15);
            //     persistentStoreOptions(s);
            //     s.UseNewtonsoftJsonSerializer();
            //     // s.UseClustering(c =>
            //     // {
            //     //     c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
            //     //     c.CheckinInterval = TimeSpan.FromSeconds(10);
            //     // });
            // });
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    }
    
    public static async Task UseCustomBackgroundJobWithQuartz<TAppJob>(this IApplicationBuilder app, string cron)
        where TAppJob: IAppJob
    {
        ISchedulerFactory schedulerFactory = app.ApplicationServices.GetRequiredService<ISchedulerFactory>();
        IScheduler scheduler = await schedulerFactory.GetScheduler();
        IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        MicroserviceMetaData microservice = configuration.GetRequiredMicroserviceMetaData();
        
        IJobDetail job = JobBuilder.Create<JobWrapper<TAppJob>>()
            .WithIdentity(nameof(TAppJob), microservice.ClientId)
            .DisallowConcurrentExecution()
            .Build();


        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(@$"{nameof(TAppJob)} Trigger", microservice.ClientId)
            .StartNow()
            .WithCronSchedule(cron)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
