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
    using CronJobs.Tasks;
    using Data;
    using ExchangeSharp;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

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
            ConfigureMvcServices(services);

            ConfigDbContext(services);

            InjectServices(services);

            ConfigExchangers(services);

            ConfigCronJobs(services);
        }

        private void ConfigDbContext(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("SQLiteConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
        }

        private void ConfigureMvcServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMemoryCache();
        }

        private void ConfigCronJobs(IServiceCollection services)
        {
            // Tasks
            services.AddSingleton<IScheduledTask, CoinInfoGrabTask>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void InjectServices(IServiceCollection services)
        {
            services.AddScoped<ICoinInfoService, CoinInfoService>();
        }

        private void ConfigExchangers(IServiceCollection services)
        {
            services.AddSingleton(typeof(ExchangeBittrexAPI));
            services.AddSingleton(typeof(ExchangeBinanceAPI));
            services.AddSingleton(typeof(ExchangeHitbtcAPI));
            //services.AddSingleton(typeof(ExchangeOkexAPI));
            services.AddSingleton(typeof(ExchangeKucoinAPI));
            services.AddSingleton(typeof(ExchangeCryptopiaAPI));
            services.AddSingleton(typeof(ExchangeYobitAPI));
            services.AddSingleton(typeof(ExchangePoloniexAPI));
        }
    }
}