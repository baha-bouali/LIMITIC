using FluentValidation;
using LIMTIC.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LIMTIC.Application.IOC
{
    public static class RegisterApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<UsersManagementService>();

            return services;
        }
    }
}
