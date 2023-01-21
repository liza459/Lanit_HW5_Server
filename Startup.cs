using Lanit_HW5_Server.Consumer;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebDataProvider;
using WebWorkersData;
using WebWorkersMapper;

namespace Lanit_HW5_Server
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
            services.AddTransient<IWorkerRepository, WorkerRepository>();
            services.AddTransient<IDbWorkersMapper, DbWorkersMapper>();
            services.AddTransient<IGetWorkerMapper, GetWorkerMapper>();
            services.AddTransient<IBranchInfoMapper, BranchInfoMapper>();
            services.AddTransient<IAchievementInfoMapper, AchievementInfoMapper>();

            services.AddControllers();

            services.AddMassTransit(mt =>
            {
                mt.AddConsumer<CreateWorkersConsumer>();
                mt.AddConsumer<GetWorkersConsumer>();
                mt.AddConsumer<UpdateWorkersConsumer>();
                mt.AddConsumer<DeleteWorkersConsumer>();

                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });

                    cfg.ReceiveEndpoint("post", ep => ep.ConfigureConsumer<CreateWorkersConsumer>(context));
                    cfg.ReceiveEndpoint("get", ep => ep.ConfigureConsumer<GetWorkersConsumer>(context));
                    cfg.ReceiveEndpoint("update", ep => ep.ConfigureConsumer<UpdateWorkersConsumer>(context));
                    cfg.ReceiveEndpoint("delete", ep => ep.ConfigureConsumer<DeleteWorkersConsumer>(context));
                });

            });
            services.AddMassTransitHostedService();
            services.AddDbContext<WorkerDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
