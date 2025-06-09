using ExchangeRate.BackgroundJobs;
using Quartz;

namespace ExchangeRate.Extensions;

public static class AddBackgroundJobsJobsExtensions
{
    public static async Task AddUpdaterJob(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var factory = provider.GetService(typeof(ISchedulerFactory));
        var conf = provider.GetService(typeof(IConfiguration));

        if (factory is not ISchedulerFactory schedulerFactory
            || conf is not IConfiguration configuration)
        {
            throw new NullReferenceException("Factory and Configuration must be provided");
        }

        // Создание планировщика
        IScheduler scheduler = await schedulerFactory.GetScheduler();
        await scheduler.Start();

        // Создание задания
        IJobDetail job = JobBuilder.Create<UpdaterJob>()
            .WithIdentity("updater_job")
            .Build();

        // Создание триггера
        var interval = configuration.GetValue<int>("Jobs:UpdaterJobIntervalSec");
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("updater_trigger")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(interval)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}