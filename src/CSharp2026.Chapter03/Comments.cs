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

namespace CSharp2026.Chapter03;

#pragma warning disable
// Chapter03/Comments.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Noise comment — the code already says this
// Increment i
i++;

// AVOID: Redundant comment — same information as the method name
// Gets the customer by ID
public Customer? GetCustomerById(Guid customerId) { ... }

// GOOD: Explains WHY a subtle decision was made
// We cap at 100 ms here because the upstream SLA is 200 ms and we
// need 100ms headroom for processing (see ADR 0042).
private static readonly TimeSpan UpstreamCallTimeout = TimeSpan.FromMilliseconds(100);

// GOOD: Documents a known limitation that cannot be fixed yet
// KNOWN ISSUE: EF Core does not translate this GroupBy to SQL in version 9.
// It materialises the entire table first. Replace with a raw SQL query
// before this endpoint is exposed to external traffic. See issue #1847.
var grouped = await _db.Orders
    .AsNoTracking()
    .GroupBy(o => o.CustomerId)
    .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);

#pragma warning restore
