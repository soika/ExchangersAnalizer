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
    using ExchangeSharp;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
            services.AddMvc();

            InjectServices(services);

            ConfigExchangers(services);
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
            services.AddSingleton(typeof(ExchangePoloniexAPI));
            services.AddSingleton(typeof(ExchangeBittrexAPI));
            services.AddSingleton(typeof(ExchangeBinanceAPI));
            services.AddSingleton(typeof(ExchangeHitbtcAPI));
            services.AddSingleton(typeof(ExchangeBithumbAPI));
            services.AddSingleton(typeof(ExchangeOkexAPI));
        }
    }
}