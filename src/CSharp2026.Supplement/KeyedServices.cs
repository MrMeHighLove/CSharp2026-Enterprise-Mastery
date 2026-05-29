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

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/KeyedServices.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/KeyedServices.cs
// .NET 8+ keyed services: different implementations by named key
public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken ct);
}

// Register multiple implementations with keys
builder.Services.AddKeyedScoped<IPaymentGateway, StripeGateway>("stripe");
builder.Services.AddKeyedScoped<IPaymentGateway, PayPalGateway>("paypal");
builder.Services.AddKeyedScoped<IPaymentGateway, BraintreeGateway>("braintree");

// Resolve by key at runtime — no factory pattern needed
public class PaymentService
{
    private readonly IServiceProvider _sp;

    public async Task<PaymentResult> ProcessAsync(Order order, string gatewayId)
    {
        var gateway = _sp.GetRequiredKeyedService<IPaymentGateway>(gatewayId);
        return await gateway.ChargeAsync(PaymentRequest.From(order));
    }
}

// Or inject directly with [FromKeyedServices] attribute
public class CheckoutService
{
    public CheckoutService(
        [FromKeyedServices("stripe")] IPaymentGateway stripeGateway,
        [FromKeyedServices("paypal")] IPaymentGateway paypalGateway)
    {
        // Both injected at construction time
    }
}

// Tenant-based gateway selection
public class TenantPaymentService
{
    private readonly IServiceProvider _sp;
    private readonly ITenantContext _tenant;

    public async Task<PaymentResult> ProcessAsync(Order order, CancellationToken ct)
    {
        var gatewayId = _tenant.Current.PaymentGateway; // "stripe", "paypal", etc.
        var gateway = _sp.GetRequiredKeyedService<IPaymentGateway>(gatewayId);
        return await gateway.ChargeAsync(PaymentRequest.From(order), ct);
    }
}

#pragma warning restore
