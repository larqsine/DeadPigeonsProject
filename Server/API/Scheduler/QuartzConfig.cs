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
                    .CronSchedule("0 0 12 ? * SUN") // Every Sunday at 12:00 noon CET to start a new game
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))));
            
            Console.WriteLine("Jobs scheduled.");
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }
}