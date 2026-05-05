using System.Text;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Settings;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Infrastructure.Data;
using LIMTIC.Infrastructure.Persistence;
using LIMTIC.Infrastructure.Repositories;
using LIMTIC.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LIMTIC.Infrastructure.IOC
{
    public static class RegisterInfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // configure refresh token settings
            services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshToken"));

            // configure jwt settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            // configure otp settings
            services.Configure<OTPTokenSettings>(configuration.GetSection("OTPToken"));
            // configure resettoken settings
            services.Configure<ResetPasswordTokenSettings>(configuration.GetSection("ResetPasswordToken"));

            // Add email settings
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            // Add email service
            services.AddScoped<IEmailService, EmailService>();

            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
         
            // register authentication middleware
            services.AddAuthentication(defaultScheme: "jwt")
                .AddJwtBearer("jwt", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Register infrastructure services
            services.AddHttpContextAccessor();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();

            return services;
        }
    }
}
