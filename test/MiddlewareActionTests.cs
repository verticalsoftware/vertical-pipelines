using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class MiddlewareActionTests
    {
        [Fact]
        public async Task InvokeAsyncPassesParameters()
        {
            var invoked = false;
            var context = "context";
            using var cts = new CancellationTokenSource();
            
            var action = new MiddlewareAction<string>((ctx, next, ct) =>
            {
                ct.ShouldBe(cts.Token);
                ctx.ShouldBe(context);
                invoked = true;
                return Task.CompletedTask;
            });

            await action.InvokeAsync(context, null!, cts.Token);
            
            invoked.ShouldBe(true);
        }
    }
}