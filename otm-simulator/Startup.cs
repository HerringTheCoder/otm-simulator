using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using otm_simulator.Helpers;
using otm_simulator.Interfaces;
using otm_simulator.Services;

namespace otm_simulator
{
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
            services.AddControllers();
            services.AddHttpClient();
            services.AddSingleton<ITimetableProvider, TimetableProviderService>();
            services.Configure<AppSettings>(Configuration);
            services.AddTransient(typeof(ITimetableAdapter<>), typeof(TimetableAdapterService<>));
            services.AddSingleton<IHostedService, BackgroundProvider>();
            services.AddSingleton<IHostedService, BackgroundWorker>();
            services.AddSingleton<IStateGenerator, StateGeneratorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
