using System.Collections.Immutable;
using AutoMapper;
using DataRetriever.Helpers;
using DataRetriever.Models;
using DataRetriever.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Recommendit.Interface;
using Recommendit.Models;
using ShowPulse.Models;
using ShowPulse.Services;

namespace DataRetriever;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RecommenditAPI", Version = "v1" });
        });

        services.AddDbContext<ShowContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ShowMappingProfile>());
        var mapper = config.CreateMapper();

        services.AddAutoMapper(typeof(ShowMappingProfile));

        services.AddTransient<IShowsRetriever, ShowsRetriever>();
        services.AddTransient<IDatabaseInserter,DatabaseInserter>();
        services.AddTransient<ITheMovieDbApiCaller, TheMovieDbApiCaller>();
        services.AddTransient<IShowService,ShowService>();
        services.AddHttpClient<ITheMovieDbApiCaller, TheMovieDbApiCaller>();

        services.Configure<TvDbSettings>(Configuration.GetSection("MovieDb"));
        services.AddSingleton<IValidateOptions<TvDbSettings>, ApiSettingsValidator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecommenditAPI V1");
            c.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}