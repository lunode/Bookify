using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bookify.Application.Users.GetLoggedInUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Bookify.Api.FunctionalTests.Users;

public class GetLoggedInUserTests : BaseFunctionalTest
{
    public GetLoggedInUserTests(FunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetProfile_Should_ReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await httpClient.GetAsync("api/v1/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_Should_ReturnUser_WhenAccessTokenIsNotMissing()
    {
        // Arrange
        string accessToken = await GetAccessToken();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        // Act
        UserResponse? user = await httpClient.GetFromJsonAsync<UserResponse>("api/v1/users/me");

        // Assert
        user.Should().NotBeNull();
    }
}
