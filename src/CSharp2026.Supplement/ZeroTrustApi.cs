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
// Supplement/ZeroTrustApi.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ZeroTrustApi.cs
// 1. JWT validation with audience and scope checking
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30) // Tighten from the 5-min default
        };
    });

// 2. Resource-based authorisation
public class OrderAuthorizationHandler
    : AuthorizationHandler<SameOwnerRequirement, Order>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx,
        SameOwnerRequirement requirement,
        Order order)
    {
        var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.CustomerId.ToString() == userId)
            ctx.Succeed(requirement);
        return Task.CompletedTask;
    }
}

// 3. Input validation with FluentValidation
public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be positive")
            .LessThanOrEqualTo(1_000_000).WithMessage("Amount exceeds limit");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .Matches(@"^[\w\s\.,\-!?]*$").WithMessage("Notes contain invalid characters");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .SetValidator(new AddressValidator());
    }
}

// 4. Endpoint combining auth, validation, and rate limiting
app.MapPut("/orders/{id}", async (
    Guid id,
    UpdateOrderRequest request,
    IValidator<UpdateOrderRequest> validator,
    IAuthorizationService auth,
    IOrderService orders,
    ClaimsPrincipal user,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(request, ct);
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

    var order = await orders.GetAsync(id, ct);
    if (order is null) return Results.NotFound();

    var authResult = await auth.AuthorizeAsync(user, order, "SameOwner");
    if (!authResult.Succeeded) return Results.Forbid();

    await orders.UpdateAsync(id, request, ct);
    return Results.NoContent();
})
.RequireAuthorization()
.RequireRateLimiting("api");

#pragma warning restore
