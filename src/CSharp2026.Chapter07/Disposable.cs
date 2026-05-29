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

namespace CSharp2026.Chapter07;

#pragma warning disable
// Chapter07/Disposable.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// GOOD: Correct IDisposable implementation with async support
public sealed class DatabaseConnectionPool : IDisposable, IAsyncDisposable
{
    private readonly SemaphoreSlim        _semaphore;
    private readonly List<DbConnection>   _connections = [];
    private bool                          _disposed;

    public DatabaseConnectionPool(int maxConnections)
        => _semaphore = new SemaphoreSlim(maxConnections, maxConnections);

    public async Task<DbConnection> AcquireAsync(CancellationToken ct)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _semaphore.WaitAsync(ct);
        return _connections.Count > 0
            ? _connections[^1]   // reuse existing
            : CreateNewConnection();
    }

    // IDisposable — synchronous cleanup
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var conn in _connections) conn.Dispose();
        _semaphore.Dispose();
    }

    // IAsyncDisposable — preferred for async resources
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var conn in _connections) await conn.DisposeAsync();
        _semaphore.Dispose();
    }

    private DbConnection CreateNewConnection() => throw new NotImplementedException();
}

// Usage with 'using declaration' (C# 8+)
await using var pool = new DatabaseConnectionPool(10);
var conn = await pool.AcquireAsync(CancellationToken.None);

#pragma warning restore
