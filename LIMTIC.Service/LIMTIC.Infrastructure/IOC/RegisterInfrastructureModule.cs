using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Abstractions.Security;
using LIMTIC.Application.Abstractions.Storage;
using LIMTIC.Application.Settings;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using LIMTIC.Infrastructure.Data;
using LIMTIC.Infrastructure.Emails;
using LIMTIC.Infrastructure.Persistence;
using LIMTIC.Infrastructure.Services;
using LIMTIC.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LIMTIC.Domain.Abstractions.AuditLogs;
using LIMTIC.Domain.Abstractions.Publications;
using LIMTIC.Domain.Abstractions.Contact;
using LIMTIC.Domain.Abstractions.Events;
using LIMTIC.Domain.Abstractions.Users;
using LIMTIC.Domain.Abstractions.ResearchAxis;
using LIMTIC.Domain.Abstractions.Settings;
using LIMTIC.Infrastructure.Repositories.AuditLogs;
using LIMTIC.Infrastructure.Repositories.Publications;
using LIMTIC.Infrastructure.Repositories.Contact;
using LIMTIC.Infrastructure.Repositories.Users;
using LIMTIC.Infrastructure.Repositories.Events;
using LIMTIC.Infrastructure.Repositories.ResearchAxis;
using LIMTIC.Infrastructure.Repositories.Settings;
using LIMTIC.Infrastructure.Repositories;
using LIMTIC.Domain.Abstractions.Files;
using LIMTIC.Infrastructure.Repositories.Files;
using LIMTIC.Domain.Abstractions;

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

            // Register data protection for encrypting sensitive settings
            services.AddDataProtection();

            // configure refresh token settings
            services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshToken"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            // configure otp settings
            services.Configure<OTPTokenSettings>(configuration.GetSection("OTPToken"));
            // configure reset token settings
            services.Configure<ResetPasswordTokenSettings>(configuration.GetSection("ResetPasswordToken"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<BlobStorageSettings>(configuration.GetSection("BlobStorage"));

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

            // repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IAuditLogsRepository, AuditLogsRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<IPublicationRepository, PublicationRepository>();
            services.AddScoped<IBookChapterRepository, BookChapterRepository>(); 
            services.AddScoped<IInternationalConferenceRepository, InternationalConferenceRepository>();
            services.AddScoped<IJournalArticleRepository, JournalArticleRepository>();
            services.AddScoped<INationalConferenceRepository, NationalConferenceRepository>();
            services.AddScoped<ITechnicalReportRepository, TechnicalReportRepository>();
            services.AddScoped<IPublicationFilesRepository, PublicationFilesRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IResearcherRepository, ResearcherRepository>();
            services.AddScoped<IPhDStudentRepository, PhDStudentRepository>();
            services.AddScoped<IMasterianRepository, MasterianRepository>();
            services.AddScoped<IResearchAxisRepository, ResearchAxisRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();

            // services
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton(sp =>
            {
                var blobSettings = configuration.GetSection("BlobStorage").Get<BlobStorageSettings>()!;
                var keyVaultUri = configuration.GetValue<string>("KeyvaultUri");
                if (string.IsNullOrWhiteSpace(keyVaultUri))
                    throw new InvalidOperationException("KeyvaultUri is not configured.");

                var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
                var secretName = blobSettings.ConnectionStringSecretName;

                var connectionString = secretClient.GetSecret(secretName).Value.Value;
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException($"Key Vault secret '{secretName}' is empty.");
                return new BlobServiceClient(connectionString);
            });
            services.AddSingleton<IBlobStorageService, BlobStorageService>();

            return services;
        }
    }
}
