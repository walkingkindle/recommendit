using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recommendit.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ShowContext>(options =>
            options.UseSqlServer(connectionString));

        // Other infrastructure-related services

        return services;
    }
    }
}
