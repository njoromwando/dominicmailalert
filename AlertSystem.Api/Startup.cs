using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertSystem.Api.QuartsJob;
using AlertSystem.Api.Service;
using AlertSystem.Api.Service.IServices;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace AlertSystem.Api
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
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableSensitiveDataLogging(false);
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), opt => opt.EnableRetryOnFailure(6));
            });
            services.AddControllers();
            services.AddTransient<IMailAlertService, MailAlertService>();
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                q.AddJob<SendAlertMail>(opts => opts.WithIdentity("SendAlertMail"));

                q.AddTrigger(opts => opts
                    .ForJob("SendAlertMail")
                    .WithIdentity("SendAlertMail-trigger")
                    .WithCronSchedule("0/30 * * * * ?"));
            });
            services.AddQuartzHostedService(
                q => q.WaitForJobsToComplete = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}