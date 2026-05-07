using LIMTIC.Application.Mappers.UserMapper;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.Application.IOC
{
    public static class MappersModule
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddScoped<IUserMapper, UserMapper>();

            return services;
        }
    }
}
