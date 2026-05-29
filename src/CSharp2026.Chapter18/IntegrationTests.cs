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

namespace CSharp2026.Chapter18;

#pragma warning disable
// Chapter18/IntegrationTests.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Integration tests against a real in-memory ASP.NET Core server
public class OrderEndpointTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public OrderEndpointTests(TestWebAppFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task CreateOrder_WithValidRequest_Returns201()
    {
        var request = new CreateOrderRequest(
            CustomerId: Guid.NewGuid().ToString(),
            Items: [new(ProductId: Guid.NewGuid().ToString(), Quantity: 1, UnitPrice: 99.99m)]);

        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server with in-memory EF Core for tests
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseInMemoryDatabase("TestDb"));

            // Replace real external services with fakes
            services.AddSingleton<IEmailSender, NullEmailSender>();
            services.AddSingleton<IPublishEndpoint, NullPublishEndpoint>();
        });
    }
}

#pragma warning restore
