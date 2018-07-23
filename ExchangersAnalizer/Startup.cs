// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the Startup.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer
{
    using System;
    using System.Linq;
    using CronJobs.Tasks;
    using Data;
    using Exchangers;
    using ExchangeSharp;
    using Extensions.Programs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Settings;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigSwagger(services);

            ConfigureMvcServices(services);

            //ConfigDbContext(services);

            InjectServices(services);

            ConfigExchangers(services);

            ConfigCustomExchangers(services);

            ConfigCronJobs(services);
        }

        private static void ConfigSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(
                c => { c.SwaggerDoc("v1", new Info {Title = "API Documentation", Version = "v1.0"}); });
        }

        private void ConfigDbContext(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("SQLiteConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
        }

        private static void ConfigureMvcServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMemoryCache();
        }

        private static void ConfigCronJobs(IServiceCollection services)
        {
            // Tasks
            services.AddSingleton<IScheduledTask, CoinInfoGrabTask>();
            services.AddSingleton<IScheduledTask, TelegramBotTask>();
            services.AddScheduler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger(
                c =>
                {
                    c.PreSerializeFilters.Add(
                        (document, request) =>
                        {
                            var paths = document.Paths.ToDictionary(
                                item => item.Key.ToLowerInvariant(),
                                item => item.Value);
                            document.Paths.Clear();
                            foreach (var pathItem in paths)
                            {
                                document.Paths.Add(pathItem.Key, pathItem.Value);
                            }
                        });
                });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation"); });
        }

        private void InjectServices(IServiceCollection services)
        {
            services.Configure<TelegramBotSettings>(Configuration.GetSection("TelegramBotSettings"));
            services.Configure<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.Configure<ExchangerEnableSettings>(Configuration.GetSection("ExchangerEnableSettings"));

            services.AddTransient<ICoinInfoService, CoinInfoService>();
            services.AddTransient<ITelegramBotService, TelegramBotService>();
        }

        private static void ConfigExchangers(IServiceCollection services)
        {
            services.AddSingleton(typeof(ExchangeBittrexAPI));
            services.AddSingleton(typeof(ExchangeBinanceAPI));
            services.AddSingleton(typeof(ExchangeHitbtcAPI));
            services.AddSingleton(typeof(ExchangeKucoinAPI));
            services.AddSingleton(typeof(ExchangeCryptopiaAPI));
            services.AddSingleton(typeof(ExchangeYobitAPI));
            services.AddSingleton(typeof(MinExchangeOkexAPI));
            services.AddSingleton(typeof(ExchangeHuobiAPI));
        }

        private static void ConfigCustomExchangers(IServiceCollection services)
        {
            services.AddHttpClient(
                "Gate",
                client => { client.BaseAddress = new Uri("https://data.gate.io/api2/1/"); });

            services.AddTransient<MinExchangeGateAPI>();

            services.AddHttpClient(
                "Houbi",
                client => { client.BaseAddress = new Uri("https://api.huobipro.com"); });

            services.AddTransient<MinExchangeHoubiAPI>();

            services.AddHttpClient(
                "Upbit",
                client => { client.BaseAddress = new Uri("https://api.upbit.com/v1/"); });

            services.AddTransient<MinExchangeUpbitAPI>();
        }
    }
}