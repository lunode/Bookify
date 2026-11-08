using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Users;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_Should_SetProperties()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        // Assert.Equal(user.FirstName, UserData.FirstName);
        // Assert.Equal(user.LastName, UserData.LastName);
        // Assert.Equal(user.Email, UserData.Email);
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
    }

    [Fact]
    public void Create_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        var userCreatedDomainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);
        userCreatedDomainEvent.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Create_Should_AddRegisteredRoleToUser()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        user.Roles.Should().Contain(Role.Registered);
    }
}
