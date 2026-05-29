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
// Chapter25/AsyncVoid.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/AsyncVoid.cs
// ANTI-PATTERN: unhandled exceptions crash the process silently
public async void ProcessOrderAsync(int orderId)
{
    var order = await _repo.GetAsync(orderId);
    await _processor.ProcessAsync(order); // If this throws, process crashes
}

// CORRECT: return Task, let the caller handle exceptions
public async Task ProcessOrderAsync(int orderId)
{
    var order = await _repo.GetAsync(orderId);
    await _processor.ProcessAsync(order);
}

// The ONLY acceptable async void: event handlers
private async void OnSubmitButton_Click(object sender, EventArgs e)
{
    try { await ProcessOrderAsync(CurrentOrderId); }
    catch (Exception ex) { ShowError(ex.Message); }
}
// Note: even here, wrap in try/catch — exceptions still uncatchable at call site

#pragma warning restore
