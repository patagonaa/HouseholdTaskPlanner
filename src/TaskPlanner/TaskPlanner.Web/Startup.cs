using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskPlanner.Web.Db;
using User.Common.Api;

namespace TaskPlanner.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRecurringTaskRepository, RecurringTaskRepository>();
            services.AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>();
            services.AddSingleton<ScheduledTaskService>();

            services.Configure<TaskPlannerDbConfiguration>(Configuration);

            services.Configure<UserApiConfiguration>(Configuration.GetSection("UserApi"));
            services.AddSingleton<IUserRemoteRepository, UserRemoteRepository>();

            services.AddHostedService<RecurringTaskScheduleInitializer>();

            services.AddResponseCompression();
            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskPlanner");
            });

            app.UseRouting();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
