using LIMTIC.Application.Abstractions;
using LIMTIC.WebAPI.Services;

namespace LIMTIC.WebAPI.IOC
{
    public static class RegisterWebApiModule
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
