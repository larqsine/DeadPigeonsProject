using Quartz;
using System;

namespace API.Scheduler;

public static class QuartzConfig
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            Console.WriteLine("Quartz Scheduler is being configured");


            q.ScheduleJob<StartNewGameJob>(trigger => trigger
                .WithIdentity("StartNewGameTrigger")
                .WithSchedule(CronScheduleBuilder
                    .CronSchedule("0 25 17 ? * THU") 
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))));

        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}