// AUTO-WRAPPED for compilation. Original snippet content follows the
// namespace declaration. Snippets are illustrative and may reference
// types that need to be supplied by the chapter or by the reader.
// See ISSUES.md for the catalog of known gaps.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.Results;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/ArchitectureTests.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ArchitectureTests.cs
using NetArchTest.Rules;

public class ArchitectureTests
{
    private const string DomainNs = "MyApp.Domain";
    private const string AppNs = "MyApp.Application";
    private const string InfrastructureNs = "MyApp.Infrastructure";

    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var result = Types.InAssembly(typeof(Domain.Order).Assembly)
            .Should().NotHaveDependencyOn(AppNs)
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join('
', result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Application.PlaceOrderCommand).Assembly)
            .Should().NotHaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join('
', result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Handlers_Should_Be_Sealed()
    {
        // MediatR handlers should not be inherited
        var result = Types.InAssembly(typeof(Application.PlaceOrderCommand).Assembly)
            .That().ImplementInterface(typeof(IRequestHandler<,>))
            .Should().BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join('
', result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Controllers_Should_Not_Contain_Business_Logic()
    {
        // Controllers should not depend on repositories directly
        var result = Types.InAssembly(typeof(WebApi.OrdersController).Assembly)
            .That().Inherit(typeof(ControllerBase))
            .Should().NotHaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join('
', result.FailingTypeNames ?? []));
    }
}

#pragma warning restore
