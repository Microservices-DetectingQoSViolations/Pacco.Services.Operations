using Convey.QoS.Violation;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Operations.Api.Infrastructure;
using Pacco.Services.Operations.Api.QoS.Job;
using Pacco.Services.Operations.Api.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Pacco.Services.Operations.Api.QoS
{
    public static class Extensions
    {
        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            return services;
        }

        public static IServiceCollection AddQoSViolationChecker(this IServiceCollection services, RequestsOptions requestsOptions)
        {
            services
                .AddSingleton<IPendingOperationsService, PendingOperationsService>()
                .AddSingleton<CheckRequestStatusJob>()
                .AddSingleton(new JobSchedule(
                    jobType: typeof(CheckRequestStatusJob),
                    cronExpression: requestsOptions.CheckRequestCronExpression));

            return services;
        }
    }
}
