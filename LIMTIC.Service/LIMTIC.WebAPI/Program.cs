using LIMTIC.Application.IOC;
using LIMTIC.Infrastructure.IOC;
using LIMTIC.WebAPI.IOC;

namespace LIMTIC.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Allow local frontend dev servers to call the API and send cookies
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowLocalDev",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:5173",
                            "http://localhost:5174",
                            "http://localhost:5175",
                            "http://localhost:5176",
                            "https://localhost:5173",
                            "https://localhost:5174",
                            "https://localhost:5175"
                        )
                              .AllowCredentials()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Services
                .AddWebApi()
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddMappers()
                .AddEndpointsApiExplorer()
                .AddSwaggerDocumentation();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowLocalDev");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}