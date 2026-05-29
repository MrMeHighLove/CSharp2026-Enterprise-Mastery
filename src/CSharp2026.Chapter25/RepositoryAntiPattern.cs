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

namespace CSharp2026.Chapter25;

#pragma warning disable
// Chapter25/RepositoryAntiPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/RepositoryAntiPattern.cs
// ANTI-PATTERN: generic repository wrapping EF Core
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Problems:
// 1. GetAllAsync() loads the entire table — no query composition
// 2. No way to express eager loading, projections, or filters
// 3. Forces EF-specific concepts through the interface anyway
// 4. Tests mock the repository, not the database — catches no query bugs

// BETTER: use DbContext directly in application services
public class OrderApplicationService
{
    private readonly OrderDbContext _db;

    public async Task<OrderDto[]> GetPendingOrdersAsync(int customerId)
    {
        return await _db.Orders
            .Where(o => o.CustomerId == customerId && o.Status == OrderStatus.Pending)
            .Select(o => new OrderDto(o.Id, o.Amount, o.CreatedAt))
            .ToArrayAsync();
    }
}

// If you need testability, use an in-memory SQLite database in tests
// It actually catches query bugs, unlike mocking IRepository

#pragma warning restore
