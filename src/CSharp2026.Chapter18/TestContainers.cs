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
// Chapter18/TestContainers.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Testcontainers: run a real PostgreSQL in Docker during tests
public class OrderRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("testdb")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        _db = new AppDbContext(opts);
        await _db.Database.MigrateAsync();
    }

    [Fact]
    public async Task SaveAsync_PersistsOrderToDatabase()
    {
        var order = OrderFactory.CreateDraftOrder();
        var repo  = new EfOrderRepository(_db, new NullDomainEventDispatcher());

        await repo.SaveAsync(order, CancellationToken.None);

        var loaded = await repo.FindByIdAsync(order.Id, CancellationToken.None);
        loaded.Should().NotBeNull();
        loaded!.Status.Should().Be(OrderStatus.Draft);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}

#pragma warning restore
