using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Vertical.Pipelines.Internal;
using Xunit;

namespace Vertical.Pipelines.Test.Internal
{
    public class MiddlewareDescriptorTests
    {
        public class NoConstructorMiddleware
        {
            private NoConstructorMiddleware(){}    
        }

        public class MultipleConstructorMiddleware
        {
            public MultipleConstructorMiddleware(){}
            public MultipleConstructorMiddleware(int _){}
        }

        public class NoPipelineDelegateParameterMiddleware
        {
            public NoPipelineDelegateParameterMiddleware(int _){}
        }

        public class PipelineDelegateOutOfOrderMiddleware
        {
            public PipelineDelegateOutOfOrderMiddleware(int _, PipelineDelegate<Request> next){}
        }

        public class NoInvokeMethodMiddleware
        {
            public NoInvokeMethodMiddleware(PipelineDelegate<Request> _){}
        }

        public class MultipleInvokeMethodMiddleware
        {
            public MultipleInvokeMethodMiddleware(PipelineDelegate<Request> _){}
            public Task Invoke(int _) => Task.CompletedTask;
            public Task Invoke(bool _) => Task.CompletedTask;
        }

        public class InvokeNoContextParameterMiddleware
        {
            public InvokeNoContextParameterMiddleware(PipelineDelegate<Request> _){}
            public Task Invoke(bool _) => Task.CompletedTask;
        }

        public class InvokeWithByRefParameterMiddleware
        {
            public InvokeWithByRefParameterMiddleware(PipelineDelegate<Request> _){}
            public Task Invoke(Request _, ref int i) => Task.CompletedTask;
        }

        public class InvokeWithNoServiceProviderContextMiddleware
        {
            public InvokeWithNoServiceProviderContextMiddleware(PipelineDelegate<Request> _) {}
            public Task Invoke(Request _, IService s) => Task.CompletedTask;
        }

        public class InvokeWithWrongReturnTypeMiddleware
        {
            public InvokeWithWrongReturnTypeMiddleware(PipelineDelegate<Request> _){}
            public bool Invoke(Request _) => true;
        }
        
        [Theory]
        [InlineData(typeof(NoConstructorMiddleware))]
        [InlineData(typeof(MultipleConstructorMiddleware))]
        [InlineData(typeof(NoPipelineDelegateParameterMiddleware))]
        [InlineData(typeof(PipelineDelegateOutOfOrderMiddleware))]
        [InlineData(typeof(NoInvokeMethodMiddleware))]
        [InlineData(typeof(MultipleInvokeMethodMiddleware))]
        [InlineData(typeof(InvokeNoContextParameterMiddleware))]
        [InlineData(typeof(InvokeWithByRefParameterMiddleware))]
        [InlineData(typeof(InvokeWithNoServiceProviderContextMiddleware))]
        [InlineData(typeof(InvokeWithWrongReturnTypeMiddleware))]
        public void ForTypeRejectsInvalidDefinition(Type type)
        {
            Should.Throw<InvalidOperationException>(() => MiddlewareDescriptor<Request>.ForType(type));
        }

        public class SimpleMiddleware
        {
            public SimpleMiddleware(PipelineDelegate<Request> _){}
            public Task Invoke(Request r) => Task.CompletedTask;
        }

        public class SimpleMiddlewareWithInvokeAsync
        {
            public SimpleMiddlewareWithInvokeAsync(PipelineDelegate<Request> _){}
            public Task InvokeAsync(Request r) => Task.CompletedTask;
        }

        [Theory]
        [InlineData(typeof(SimpleMiddleware))]
        [InlineData(typeof(SimpleMiddlewareWithInvokeAsync))]
        public void ForTypeCreatesDescriptor(Type type)
        {
            Should.NotThrow(() => MiddlewareDescriptor<Request>.ForType(type));
        }

        public class MiddlewareWithLifetimeDependency
        {
            internal readonly PipelineDelegate<Request> Next;
            internal readonly IService Service;

            public MiddlewareWithLifetimeDependency(
                PipelineDelegate<Request> next,
                IService service)
            {
                Next = next;
                Service = service;
            }

            public Task InvokeAsync(Request request)
            {
                request.Count++;
                Service.Call();
                return Next(request);
            }
        }

        [Fact]
        public void CreateInstanceInjectsLifetimeDependencies()
        {
            var service = Substitute.For<IService>();
            var descriptor = MiddlewareDescriptor<Request>.ForType(typeof(MiddlewareWithLifetimeDependency));
            var instance = descriptor.CreateInstance(_ => Task.CompletedTask, new object?[]{service});
            ((MiddlewareWithLifetimeDependency) instance).Service.ShouldBe(service);
        }

        [Fact]
        public void CreateInstanceInjectsNextDelegate()
        {
            PipelineDelegate<Request> next = _ => Task.CompletedTask;;
            var descriptor = MiddlewareDescriptor<Request>.ForType(typeof(MiddlewareWithLifetimeDependency));
            var instance = descriptor.CreateInstance(next, new object?[] { Substitute.For<IService>() });
            ((MiddlewareWithLifetimeDependency) instance).Next.ShouldBe(next);
        }

        [Fact]
        public void CreateInstanceInjectsServiceProviderDependencies()
        {
            PipelineDelegate<Request> next = _ => Task.CompletedTask;;
            var serviceProvider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IService>();
            serviceProvider.GetService(typeof(IService)).Returns(service);
            var instance = (MiddlewareWithLifetimeDependency) 
                MiddlewareDescriptor<Request>.ForType(typeof(MiddlewareWithLifetimeDependency))
                .CreateInstance(next, new object?[] { serviceProvider });
            instance.Service.ShouldBe(service);
        }

        public class MiddlewareWithScopeDependencies
        {
            private readonly PipelineDelegate<RequestWithServiceProvider> _next;

            public MiddlewareWithScopeDependencies(PipelineDelegate<RequestWithServiceProvider> next)
            {
                _next = next;
            }

            public Task InvokeAsync(RequestWithServiceProvider request, IService service)
            {
                request.Count++;
                service.Call();
                return _next(request);
            }
        }

        [Fact]
        public async Task CompileHandlerInjectsServices()
        {
            var service = Substitute.For<IService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IService)).Returns(service);
            var request = new RequestWithServiceProvider(serviceProvider);
            var descriptor = MiddlewareDescriptor<RequestWithServiceProvider>.ForType(typeof(MiddlewareWithScopeDependencies));
            var instance = descriptor.CreateInstance(r =>
            {
                r.Count++;
                return Task.CompletedTask;
            }, Array.Empty<object?>());
            var handler = descriptor.CompileHandler();

            await handler(instance, request);

            request.Count.ShouldBe(2);
            serviceProvider.Received().GetService(typeof(IService));
            service.Received().Call();
        }
    }
}