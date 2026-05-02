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

            // Add controllers to the container.
            builder.Services.AddControllers();

            // Add application & infrastructure services
            builder.Services
                .AddWebApi()
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddMappers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
