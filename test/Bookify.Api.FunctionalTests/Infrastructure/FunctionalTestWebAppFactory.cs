using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Users;
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

namespace Bookify.Api.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
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
        await IntializeAsync();
    }

    private async Task IntializeAsync()
    {
        var httpClient = CreateClient();
        await httpClient.PostAsJsonAsync("/api/v1/users/register", UserData.RegisterTestUserRequest);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await dbContainer.StopAsync();
        await redisContainer.StopAsync();
        await keycloakContainer.StopAsync();
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

            // Bookify.Infrastructure InternalsVisibleTo Bookify.Api.FunctionalTests
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

            services.Configure<AuthenticationOptions>(options =>
            {
                options.Issuer = $"{keycloakAddr}realms/bookify";
                options.MetadataUrl = $"{keycloakAddr}realms/bookify/.well-known/openid-configuration";
            });
        });
    }
}