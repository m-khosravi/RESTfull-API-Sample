using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YalitApi.Filters;
using YalitApi.Infrastructure;
using YalitApi.Models;
using Microsoft.EntityFrameworkCore;
using YalitApi.Services;

namespace YalitApi
{
    public class Startup
    {
        private readonly int? _httpsPort;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            if (env.IsDevelopment())
            {
                var launchJasonConfig = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Properties\\launchSettings.json")
                    .Build();

                _httpsPort = launchJasonConfig.GetValue<int>("iisSettings:iisExpress:sslPort");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Use an in-memory database for quick dev and testing
            // TODO: Swap out with real database in production
            services.AddDbContext<HotelApiContext>(opt => opt.UseInMemoryDatabase("dbHotelApp"));

            services.AddMvc(opt => 
            {
                opt.Filters.Add(typeof(JsonExceptionFilter));

                // Require Https for all controllers
                opt.SslPort = _httpsPort;
                opt.Filters.Add(typeof(RequireHttpsAttribute));

                var jsonFormatter = opt.OutputFormatters.OfType<JsonOutputFormatter>().Single();
                opt.OutputFormatters.Remove(jsonFormatter);

                opt.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));
            });

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new MediaTypeApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            });

            services.Configure<HotelInfo>(Configuration.GetSection("Info"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<HotelInfo>>().Value);

            services.AddScoped<IRoomService, DefaultRoomService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                var context = app.ApplicationServices.GetRequiredService<HotelApiContext>();
                AddTestData(context);

            }
            app.UseHsts(opt =>
            {
                opt.MaxAge(days: 365);
                opt.IncludeSubdomains();
                opt.Preload();
            });
            app.UseMvc();
        }

        private static void AddTestData(HotelApiContext context)
        {
            context.Rooms.Add(new RoomEntity() {
                Id = Guid.NewGuid(),
                Name = "Room Num 14",
                Rate = 10119,
            });

            context.Rooms.Add(new RoomEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Room Num 24",
                Rate = 123000,
            });
        }
    }
}
