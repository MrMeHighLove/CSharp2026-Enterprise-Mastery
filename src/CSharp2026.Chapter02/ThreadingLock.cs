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

namespace CSharp2026.Chapter02;

#pragma warning disable
// Chapter02/ThreadingLock.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Old approach — using an opaque object as a mutex
private readonly object _gate = new object();

void OldWay()
{
    lock (_gate) { /* critical section */ }
}

// C# 13 / .NET 9+ — use System.Threading.Lock
private readonly Lock _lock = new Lock();

void NewWay()
{
    lock (_lock) { /* critical section — more efficient */ }
}

// Also supports explicit Enter/Exit for try/finally patterns:
using (_lock.EnterScope())
{
    // critical section
}

#pragma warning restore
