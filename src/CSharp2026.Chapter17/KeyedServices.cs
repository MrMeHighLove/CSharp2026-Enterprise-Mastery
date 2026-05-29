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

namespace CSharp2026.Chapter17;

#pragma warning disable
// Chapter17/KeyedServices.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// .NET 8+ Keyed Services: resolve different implementations by key
public interface IStorageProvider
{
    Task UploadAsync(string path, Stream content, CancellationToken ct);
}

public class AzureBlobStorage : IStorageProvider { ... }
public class LocalDiskStorage  : IStorageProvider { ... }
public class S3Storage         : IStorageProvider { ... }

// Register with keys
builder.Services.AddKeyedSingleton<IStorageProvider, AzureBlobStorage>("azure");
builder.Services.AddKeyedSingleton<IStorageProvider, LocalDiskStorage>("local");
builder.Services.AddKeyedSingleton<IStorageProvider, S3Storage>("s3");

// Inject by key via [FromKeyedServices] attribute
public class DocumentService
{
    private readonly IStorageProvider _storage;

    public DocumentService(
        [FromKeyedServices("azure")] IStorageProvider storage)
        => _storage = storage;
}

// Or resolve dynamically at runtime
public class StorageRouter
{
    private readonly IServiceProvider _sp;
    public StorageRouter(IServiceProvider sp) => _sp = sp;

    public IStorageProvider GetFor(string provider)
        => _sp.GetRequiredKeyedService<IStorageProvider>(provider);
}

#pragma warning restore
