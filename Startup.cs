using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Recommendit.Interface;
using ShowPulse.Models;
using ShowPulse.Services;

namespace Recommendit
{
     public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ShowContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("ConnectionStrings:DefaultConnection")));
            services.AddTransient<IShowService, ShowService>();
            services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RecommenditAPI", Version = "v1" });
        });
          
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Api V1");
            });

        }
    }
}
  