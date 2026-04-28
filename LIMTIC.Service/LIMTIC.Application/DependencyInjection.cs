using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LIMTIC.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
