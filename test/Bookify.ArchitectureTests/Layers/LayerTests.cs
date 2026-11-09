using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_Should_Not_HaveDependencyOn_ApplicationLayer()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainLayer_Should_Not_HaveDependencyOn_InfrastructureLayer()
    {
        // Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }


    [Fact]
    public void ApplicationLayer_Should_Not_HaveDependencyOn_InfrastructureLayer()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_Should_Not_HaveDependencyOn_PresentationLayer()
    {
        // Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_Should_Not_HaveDependencyOn_PresentationLayer()
    {
        // Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}