using System;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class Invoker<TContext, TResult>
    {
        private readonly object _middleware;

        public Invoker(object middleware)
        {
            _middleware = middleware;
        }

        public TResult Invoke(TContext context, IServiceProvider serviceProvider)
        {
            return default!;
        }
    }
    
    public class Tests
    {
        [Fact]
        public async Task ScrewAround()
        {
            var context = new TestContext();
        }
    }
}