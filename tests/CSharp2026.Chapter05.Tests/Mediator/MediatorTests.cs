using CSharp2026.Chapter05.Mediator;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CSharp2026.Chapter05.Tests.Mediator;

public sealed class MediatorTests
{
    public sealed record SumRequest(int A, int B) : IRequest<int>;

    public sealed class SumHandler : IRequestHandler<SumRequest, int>
    {
        public Task<int> HandleAsync(SumRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return Task.FromResult(request.A + request.B);
        }
    }

    [Fact]
    public async Task Mediator_Resolves_And_Invokes_Handler()
    {
        var services = new ServiceCollection()
            .AddSimpleMediator()
            .AddScoped<IRequestHandler<SumRequest, int>, SumHandler>()
            .BuildServiceProvider();

        var mediator = services.GetRequiredService<IMediator>();
        int result = await mediator.SendAsync(new SumRequest(2, 3), CancellationToken.None);

        result.Should().Be(5);
    }

    [Fact]
    public async Task Mediator_Throws_For_Unregistered_Request()
    {
        var services = new ServiceCollection().AddSimpleMediator().BuildServiceProvider();
        var mediator = services.GetRequiredService<IMediator>();

        var act = async () => await mediator.SendAsync(new SumRequest(1, 1), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*SumRequest*");
    }
}
