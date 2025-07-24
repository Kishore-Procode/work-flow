using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkflowMgmt.Domain.Interface.Common;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Interface.JwtToken;
using WorkflowMgmt.Infrastructure.ConnectionFactory;
using WorkflowMgmt.Infrastructure.Services;
using WorkflowMgmt.Infrastructure.TypeHandlers;
using Dapper;

namespace WorkflowMgmt.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Configure PostgreSQL array type mapping for Dapper
            SqlMapper.AddTypeHandler(new IntArrayTypeHandler());

            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            //services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
            return services;
        }
    }
}
