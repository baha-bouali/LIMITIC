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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}