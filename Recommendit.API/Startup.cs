using AutoMapper;
using DataRetriever;
using DataRetriever.Models;
using DataRetriever.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Recommendit.Common.Helpers;
using Recommendit.Infrastructure;
using Recommendit.Interface;
using Recommendit.Models;
using ShowPulse.Engine;
using ShowPulse.Services;

namespace Recommendit
{
     public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

            services.AddInfrastructure(_configuration);
            services.AddTransient<IShowService, ShowService>();
            services.AddTransient<IShowsRetriever, ShowsRetriever>();
            services.AddTransient<IDatabaseOperator, DatabaseOperator>();
            services.AddTransient<ITheMovieDbApiCaller, TheMovieDbApiCaller>();

            services.AddTransient<IVectorService, VectorAIService>();

            services.AddHttpClient<ITheMovieDbApiCaller, TheMovieDbApiCaller>();

            services.AddHttpClient<IShowsRetriever, ShowsRetriever>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200");
                    });
            });

            services.Configure<TvDbSettings>(_configuration.GetSection("MovieDb"));
            services.AddSingleton<IValidateOptions<TvDbSettings>, ApiSettingsValidator>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<ShowMappingProfile>());
            var mapper = config.CreateMapper();

            services.AddAutoMapper(typeof(ShowMappingProfile));

            services.AddControllers();
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
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Api V1");
                });
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
  