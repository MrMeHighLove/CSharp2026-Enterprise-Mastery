using FluentAssertions;
using Xunit;

namespace CSharp2026.Chapter14.Tests;

public sealed class AssemblyLoadsTests
{
    [Fact]
    public void Assembly_Loads()
    {
        var asm = typeof(CSharp2026.Chapter14.Tests.AssemblyLoadsTests).Assembly;
        asm.Should().NotBeNull();
    }
}
