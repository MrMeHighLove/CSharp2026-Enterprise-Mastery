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

namespace CSharp2026.Chapter24;

#pragma warning disable
// Chapter24/AsyncMigration.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter24/AsyncMigration.cs
// LEGACY: sync, blocking, thread-hungry
public class OrderService
{
    public Order GetOrder(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();  // Blocks a thread
        return conn.QuerySingle<Order>("SELECT * FROM Orders WHERE Id = @id", new { id });
    }
}

// STEP 1: Add async overload, keep sync for now (don't break callers)
public class OrderService
{
    public Order GetOrder(int id) => GetOrderAsync(id).GetAwaiter().GetResult();

    public async Task<Order> GetOrderAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        return await conn.QuerySingleAsync<Order>(
            "SELECT * FROM Orders WHERE Id = @id", new { id });
    }
}

// STEP 2: Migrate callers to async; remove sync overload
// STEP 3: Fix any ConfigureAwait(false) in library code
// STEP 4: Remove all .Result and .GetAwaiter().GetResult() calls

// WARNING: Never call .Result on a Task in ASP.NET — it deadlocks
// The async migration must be bottom-up: data layer first, then services, then controllers

#pragma warning restore
