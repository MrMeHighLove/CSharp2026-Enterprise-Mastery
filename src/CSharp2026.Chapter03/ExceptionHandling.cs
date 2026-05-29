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
// Chapter03/ExceptionHandling.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Swallowed exception — the worst pattern in production code
try
{
    await _paymentGateway.ChargeAsync(order, ct);
}
catch (Exception)
{
    // Nothing here — caller has no idea if the charge succeeded
}

// AVOID: Overly broad catch at the wrong layer
try
{
    var result = await ProcessComplexWorkflowAsync(ct);
    return result;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Workflow failed");
    return null!; // caller doesn't know what 'null' means
}

// GOOD: Catch what you can handle; let everything else propagate
public async Task<PaymentResult> ChargeCustomerAsync(Order order, CancellationToken ct)
{
    try
    {
        return await _paymentGateway.ChargeAsync(order, ct);
    }
    catch (PaymentDeclinedException ex)
    {
        // We know how to handle declines — return a typed result
        _logger.LogInformation("Payment declined for order {OrderId}: {Reason}",
            order.Id, ex.DeclineReason);
        return PaymentResult.Declined(ex.DeclineReason);
    }
    catch (PaymentGatewayUnavailableException)
    {
        // Transient failure — let the caller or the retry policy handle it
        throw;
    }
    // Any other exception propagates to the global handler
}

#pragma warning restore
