using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Application.IntergrationTests.Infrastructure;

public class IntergrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer =
        new PostgreSqlBuilder("postgres:17.2")
            .WithDatabase("bookify")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private readonly RedisContainer redisContainer =
        new RedisBuilder("redis:7-alpine")
            .Build();

    // default to start-dev command
    private readonly KeycloakContainer keycloakContainer =
        new KeycloakBuilder("keycloak/keycloak:26.8")
            .WithRealm(".files/bookify-realm-export.json")
            .Build();

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();
        await redisContainer.StartAsync();
        await keycloakContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            services.AddDbContext<ApplicationDbContext>(
                options => options
                    .UseNpgsql(dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            services.RemoveAll(typeof(ISqlConnectionFactory));

            // Bookify.Application.IntergrationTests InternalsVisibleTo
            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(dbContainer.GetConnectionString()));

            services.Configure<RedisCacheOptions>(options =>
                options.Configuration = redisContainer.GetConnectionString());

            // example: http://localhost:8080/
            var keycloakAddr = keycloakContainer.GetBaseAddress();

            services.Configure<KeycloakOptions>(options =>
            {
                options.AdminUrl = $"{keycloakAddr}admin/realms/bookify/";
                options.TokenUrl = $"{keycloakAddr}realms/bookify/protocol/openid-connect/token";
            });
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await dbContainer.StopAsync();
        await redisContainer.StopAsync();
        await keycloakContainer.StopAsync();
    }
}