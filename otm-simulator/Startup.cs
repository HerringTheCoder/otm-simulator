using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using otm_simulator.Helpers;
using otm_simulator.Hubs;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8000",
                                            "http://localhost:8080");
                    });
            });
            services.AddControllers();
            services.AddHttpClient();
            services.Configure<AppSettings>(Configuration);
            services.AddSignalR();
            services.AddScoped<ITimeProvider, TimeProvider>();
            services.AddScoped(typeof(ITimetableAdapter<>), typeof(TimetableAdapterService<>));
            services.AddSingleton<IHostedService, BackgroundProvider>();
            services.AddSingleton<ITimetableProvider, TimetableProviderService>();
            services.AddSingleton<IStateGenerator, StateGeneratorService>();
            services.AddHostedService<BackgroundWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<StatesHub>("/hubs/states");
            });
        }
    }
}
