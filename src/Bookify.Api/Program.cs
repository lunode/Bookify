using Bookify.Api.Controllers.Apartments;
using Bookify.Api.Controllers.Bookings;
using Bookify.Api.Controllers.Reviews;
using Bookify.Api.Controllers.Users;
using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigurationSwaggerOptions>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
    app.ApplyMigrations();
    app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLoggingMiddleware();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// app.MapApartmentsEndpoints();
// app.MapReviewEndpoints();
// app.MapUsersEndpoints();
// Asp.Versioning.Builder.ApiVersionSet apiVersionSet = app.NewApiVersionSet()
//     .HasApiVersion(new Asp.Versioning.ApiVersion(1))
//     .ReportApiVersions()
//     .Build();
// var routeGroupBuilder = app.MapGroup("/api/v{version:apiVersion}").WithApiVersionSet(apiVersionSet);
// routeGroupBuilder.MapBookingsEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();