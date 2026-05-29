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
// Chapter07/MemoryDiagnostics.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

# Collect a 30-second allocation trace from a running process
dotnet-trace collect --process-id <PID> \
  --providers Microsoft-Windows-DotNETRuntime:0x1:5 \
  --duration 00:00:30

# Dump a heap snapshot for offline analysis
dotnet-dump collect --process-id <PID>

# Analyse a dump: find the top allocated types
dotnet-dump analyze <dump-file>
> dumpheap -stat
> dumpheap -type System.String -min 10000   # strings > 10KB

# In code: monitor GC via EventListener (production-safe)
var gcInfo = GC.GetGCMemoryInfo();
var heapSizeMb = gcInfo.HeapSizeBytes / 1_048_576.0;
var gen2CollectCount = GC.CollectionCount(2);

#pragma warning restore
