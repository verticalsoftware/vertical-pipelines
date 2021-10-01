using System;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class Tests
    {
        [Fact]
        public async Task ScrewAround()
        {
            var context = new TestContext();
            var serviceProvider = Substitute.For<IServiceProvider>();

            var c = new MiddlewareC(_ => Task.CompletedTask);
            var cWrapper = new Func<TestContext, IServiceProvider, Task>((ctx, sp) => c.InvokeAsync(ctx));
            
            var b = new MiddlewareB(ctx => cWrapper(ctx, serviceProvider));
            var bWrapper = new Func<TestContext, IServiceProvider, Task>((ctx, sp) => b.InvokeAsync(ctx));
            
            var a = new MiddlewareA(ctx => bWrapper(ctx, serviceProvider));
            var aWrapper = new Func<TestContext, IServiceProvider, Task>((ctx, sp) => a.InvokeAsync(ctx));

            await aWrapper(context, serviceProvider);
            
            context.Count.ShouldBe(3);
        }
    }
}