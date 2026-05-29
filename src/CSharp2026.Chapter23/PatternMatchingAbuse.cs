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

namespace CSharp2026.Chapter23;

#pragma warning disable
// Chapter23/PatternMatchingAbuse.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter23/PatternMatchingAbuse.cs
// PROBLEMATIC: pattern matching scattered across the codebase
// Every place that uses Shape must know about all concrete types
decimal CalculateArea(Shape shape) => shape switch
{
    Circle c => Math.PI * c.Radius * c.Radius,
    Rectangle r => r.Width * r.Height,
    Triangle t => 0.5m * t.Base * t.Height,
    _ => throw new NotSupportedException()
};

string Describe(Shape shape) => shape switch
{
    Circle c when c.Radius > 10 => "Large circle",
    Circle => "Small circle",
    Rectangle r when r.Width == r.Height => "Square",
    Rectangle => "Rectangle",
    _ => "Unknown"
};

// BETTER: polymorphism — new shapes require changes in one place only
public abstract class Shape
{
    public abstract decimal Area { get; }
    public abstract string Describe();
}

public class Circle(decimal radius) : Shape
{
    public override decimal Area => (decimal)Math.PI * radius * radius;
    public override string Describe() => radius > 10 ? "Large circle" : "Small circle";
}

#pragma warning restore
