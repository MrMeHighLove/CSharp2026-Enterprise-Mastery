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

namespace CSharp2026.Reference;

#pragma warning disable
// Reference/EfCoreConfig.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/EfCoreConfig.cs
// IEntityTypeConfiguration — keep entity configuration out of DbContext
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever(); // Domain generates IDs

        builder.Property(o => o.Status)
            .HasConversion<string>() // Store as string, not integer
            .HasMaxLength(50);

        builder.Property(o => o.Amount)
            .HasPrecision(18, 2);

        // Value object as owned entity
        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street).HasMaxLength(200);
            addr.Property(a => a.City).HasMaxLength(100);
            addr.Property(a => a.PostalCode).HasMaxLength(20);
        });

        // One-to-many: eager loading
        builder.HasMany(o => o.Lines)
            .WithOne()
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for common query patterns
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => new { o.Status, o.CreatedAt });
        builder.HasIndex(o => o.CreatedAt);

        // Row version for optimistic concurrency
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}

// DbContext: apply all configurations from assembly
protected override void OnModelCreating(ModelBuilder model)
{
    model.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    // Convention: all string properties default to varchar(255) not nvarchar(max)
    foreach (var prop in model.Model.GetEntityTypes()
        .SelectMany(e => e.GetProperties())
        .Where(p => p.ClrType == typeof(string)))
    {
        if (prop.GetMaxLength() == null)
            prop.SetMaxLength(255);
    }
}

#pragma warning restore
