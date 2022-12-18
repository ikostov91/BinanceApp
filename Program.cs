using BinanceWebsocketApp.Contracts;
using BinanceWebsocketApp.Persistance;
using BinanceWebsocketApp.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace BinanceWebsocketApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseWebSockets();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddScoped<IBinanceService, BinanceService>();
            builder.Services.AddSingleton<IHostedService, BackgroundDataCollectionService>();
            builder.Services.AddControllers((options) =>
            {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
                options.Filters.Add(new ProducesAttribute("application/json", "application/xml"));
                options.Filters.Add(new ConsumesAttribute("application/json", "application/xml"));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMemoryCache();
        }
    }
}