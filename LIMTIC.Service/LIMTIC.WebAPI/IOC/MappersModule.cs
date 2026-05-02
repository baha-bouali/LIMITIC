using LIMTIC.WebAPI.Mappers.UserMapper;

namespace LIMTIC.WebAPI.IOC
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
