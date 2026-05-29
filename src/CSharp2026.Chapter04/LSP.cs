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

namespace CSharp2026.Chapter04;

#pragma warning disable
// Chapter04/LSP.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Classic LSP violation: Rectangle / Square
public class Rectangle
{
    public virtual int Width  { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public class Square : Rectangle
{
    public override int Width  { set => base.Width = base.Height = value; }
    public override int Height { set => base.Width = base.Height = value; }
}

// This method works for Rectangle but breaks for Square:
static void AssertRectangleArea(Rectangle r)
{
    r.Width  = 5;
    r.Height = 4;
    Debug.Assert(r.Area == 20); // FAILS for Square — Area is 16
}

// GOOD: LSP-safe design: model shapes without inheritance
public abstract record Shape
{
    public abstract int Area { get; }
}

public record RectangleShape(int Width, int Height) : Shape
{
    public override int Area => Width * Height;
}

public record SquareShape(int Side) : Shape
{
    public override int Area => Side * Side;
}

#pragma warning restore
