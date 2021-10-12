using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class PipelineBuilderTests
    {
        private readonly Request _request = new Request();
        
        [Fact]
        public async Task UseRegistersDelegate()
        {
            var pipeline = new PipelineBuilder<Request>()
                .Use(next => request =>
                {
                    request.Count++;
                    return next(request);
                })
                .Build();

            await pipeline(_request);
            _request.Count.ShouldBe(1);
        }

        public class MiddlewareA
        {
            private readonly PipelineDelegate<Request> _next;

            public MiddlewareA(PipelineDelegate<Request> next)
            {
                _next = next;
            }

            public Task InvokeAsync(Request request)
            {
                request.Strings.Add(nameof(MiddlewareA));
                return _next(request);
            }
        }
        
        public class MiddlewareB
        {
            private readonly PipelineDelegate<Request> _next;

            public MiddlewareB(PipelineDelegate<Request> next)
            {
                _next = next;
            }

            public Task InvokeAsync(Request request)
            {
                request.Strings.Add(nameof(MiddlewareB));
                return _next(request);
            }
        }

        public class ShortCircuitMiddleware
        {
            private readonly PipelineDelegate<Request> _next;

            public ShortCircuitMiddleware(PipelineDelegate<Request> next)
            {
                _next = next;
            }

            public Task InvokeAsync(Request request)
            {
                request.Strings.Add(nameof(ShortCircuitMiddleware));
                return Task.CompletedTask;
            }
        }

        public class ServiceDependencyMiddleware
        {
            private readonly PipelineDelegate<Request> _next;
            private readonly IService _service;

            public ServiceDependencyMiddleware(PipelineDelegate<Request> next, IService service)
            {
                _next = next;
                _service = service;
            }

            public Task InvokeAsync(Request request)
            {
                _service.Call();
                return Task.CompletedTask;
            }
        }
        
        [Fact]
        public async Task UseRegistersMiddleware()
        {
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware(typeof(MiddlewareA))
                .UseMiddleware(typeof(MiddlewareB))
                .Build();

            await pipeline(_request);
            
            _request.Strings[0].ShouldBe(nameof(MiddlewareA));
            _request.Strings[1].ShouldBe(nameof(MiddlewareB));
        }
        
        [Fact]
        public async Task UseRegistersMiddlewareWithGeneric()
        {
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware<MiddlewareA>()
                .UseMiddleware<MiddlewareB>()
                .Build();

            await pipeline(_request);
            
            _request.Strings[0].ShouldBe(nameof(MiddlewareA));
            _request.Strings[1].ShouldBe(nameof(MiddlewareB));
        }

        public class MiddlewareWithParameter
        {
            private readonly PipelineDelegate<Request> _next;

            public MiddlewareWithParameter(PipelineDelegate<Request> next, Action invokeMe)
            {
                _next = next;
                invokeMe();
            }

            public Task InvokeAsync(Request request) => _next(request);
        }

        [Fact]
        public async Task UseMiddlewarePassesArgs()
        {
            var invoked = false;
            void InvokeMe() => invoked = true;
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware(typeof(MiddlewareWithParameter), (Action)InvokeMe)
                .Build();

            await pipeline(_request);

            invoked.ShouldBe(true);
        }

        [Fact]
        public async Task UseMiddlewareInvokesServiceProvider()
        {
            var service = Substitute.For<IService>();
            var provider = Substitute.For<IServiceProvider>();
            provider.GetService(typeof(IService)).Returns(service);
            provider.GetService(typeof(Action)).Returns(new Action(() => {}));
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware(typeof(ServiceDependencyMiddleware), provider)
                .Build();

            await pipeline(_request);
            service.Received().Call();
        }
        
        [Fact]
        public async Task UseMiddlewareOfTInvokesServiceProvider()
        {
            var service = Substitute.For<IService>();
            var provider = Substitute.For<IServiceProvider>();
            provider.GetService(typeof(IService)).Returns(service);
            provider.GetService(typeof(Action)).Returns(new Action(() => {}));
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware<ServiceDependencyMiddleware>(provider)
                .Build();

            await pipeline(_request);
            service.Received().Call();
        }

        [Fact]
        public async Task UseShortCircuitMiddlewareShortCircuits()
        {
            var pipeline = new PipelineBuilder<Request>()
                .UseMiddleware<ShortCircuitMiddleware>()
                .UseMiddleware<MiddlewareA>()
                .UseMiddleware<MiddlewareB>()
                .Build();

            await pipeline(_request);
            
            _request.Strings.Single().ShouldBe(nameof(ShortCircuitMiddleware));
        }
    }
}