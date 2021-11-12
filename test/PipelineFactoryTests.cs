using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class PipelineFactoryTests
    {
        [Fact]
        public async Task ExecutePipeline_Invokes_Components()
        {
            var middlewares = new IPipelineMiddleware<List<string>>[]
            {
                new MiddlewareAction<List<string>>((list, next, cancel) =>
                {
                    list.Add("red");
                    return next(list, cancel);
                }),
                new MiddlewareAction<List<string>>((list, next, cancel) =>
                {
                    list.Add("green");
                    return next(list, cancel);
                }),
                new MiddlewareAction<List<string>>((list, next, cancel) =>
                {
                    list.Add("blue");
                    return next(list, cancel);
                }),
            };

            var factory = new PipelineFactory<List<string>>(middlewares);

            var context = new List<string>();

            await factory.CreatePipeline()(context, CancellationToken.None);
            
            context[0].ShouldBe("red");
            context[1].ShouldBe("green");
            context[2].ShouldBe("blue");
        }
        
        [Fact]
        public async Task ExecutePipeline_Propagates_Context()
        {
            var middleware = Substitute.For<IPipelineMiddleware<string>>();
            middleware.InvokeAsync("test", Arg.Any<PipelineDelegate<string>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var context = callInfo.ArgAt<string>(0);
                    var next = callInfo.ArgAt<PipelineDelegate<string>>(1);
                    var cancelToken = callInfo.ArgAt<CancellationToken>(2);

                    return next(context, cancelToken);
                });

            var pipeline = new PipelineFactory<string>(new[] { middleware }).CreatePipeline();

            await pipeline("test", CancellationToken.None);

            await middleware.Received(1).InvokeAsync("test", Arg.Any<PipelineDelegate<string>>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecutePipeline_Propagates_CancellationToken()
        {
            using var cancellationSource = new CancellationTokenSource();
            var cancellationToken = cancellationSource.Token;

            var middleware = Substitute.For<IPipelineMiddleware<string>>();
            middleware.InvokeAsync(Arg.Any<string>(), Arg.Any<PipelineDelegate<string>>(), cancellationToken)
                .Returns(callInfo =>
                {
                    var context = callInfo.ArgAt<string>(0);
                    var next = callInfo.ArgAt<PipelineDelegate<string>>(1);
                    var cancelToken = callInfo.ArgAt<CancellationToken>(2);

                    return next(context, cancelToken);
                });

            var pipeline = new PipelineFactory<string>(new[] { middleware }).CreatePipeline();

            await pipeline("test", cancellationToken);

            await middleware.Received(1).InvokeAsync(Arg.Any<string>(), Arg.Any<PipelineDelegate<string>>(), cancellationToken);
        }
    }
}