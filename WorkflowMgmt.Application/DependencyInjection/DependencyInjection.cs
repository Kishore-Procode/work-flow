using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WorkflowMgmt.Application.Common.Behaviours;
using WorkflowMgmt.Application.Services;
using WorkflowMgmt.Domain.Interface.IServices;

namespace WorkflowMgmt.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register application services
            services.AddScoped<IWorkflowActionProcessorService, WorkflowActionProcessorService>();
            services.AddScoped<IWorkflowValidationService, WorkflowValidationService>();
            services.AddScoped<IWorkflowNotificationService, WorkflowNotificationService>();

            return services;
        }
    }
}
