using LIMTIC.Application.Settings;
using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly int? _mailHogSmtpPort;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public CustomWebApplicationFactory(string connectionString, int mailHogSmtpPort)
    {
        _connectionString = connectionString;
        _mailHogSmtpPort = mailHogSmtpPort;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            if (_mailHogSmtpPort.HasValue)
            {
                RegisterTestEmailSettings(services, _mailHogSmtpPort.Value);
            }
        });
    }
    private void RegisterTestEmailSettings(IServiceCollection services, int smtpPort)
    {
        var emailDescriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IOptions<EmailSettings>));
        if (emailDescriptor != null)
            services.Remove(emailDescriptor);

        services.AddSingleton<IOptions<EmailSettings>>(Options.Create(new EmailSettings
        {
            SmtpHost = "localhost",
            SmtpPort = smtpPort,
            SenderEmail = "test@test.com",
            SenderName = "Test",
            Password = "",
            EnableSsl = false
        }));
    }
}