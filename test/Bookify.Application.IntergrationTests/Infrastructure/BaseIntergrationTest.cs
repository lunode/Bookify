using Bookify.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application.IntergrationTests.Infrastructure;

public abstract class BaseIntergrationTest : IClassFixture<IntergrationTestWebAppFactory>
{
    protected readonly IServiceScope serviceScope;

    protected readonly ISender sender;

    protected readonly ApplicationDbContext dbContext;

    protected BaseIntergrationTest(IntergrationTestWebAppFactory factory)
    {
        serviceScope = factory.Services.CreateScope();
        sender = serviceScope.ServiceProvider.GetRequiredService<ISender>();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    }
}