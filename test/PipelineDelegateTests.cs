using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Vertical.Pipelines.Test
{
    public class PipelineDelegateTests
    {
        [Fact]
        public void ConstructWithNullTasksThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new PipelineDelegate<object>(null!));
        }

        [Fact]
        public async Task InvokeAsyncCallsFirstTask()
        {
            var task = Substitute.For<IPipelineTask<object?>>();
            var testDelegate = new PipelineDelegate<object?>(new[] {task});

            await testDelegate.InvokeAsync(null, CancellationToken.None);

            await task.Received(1).InvokeAsync(null, testDelegate, CancellationToken.None);
        }

        [Fact]
        public async Task InvokeAllAsyncCallsFirstTask()
        {
            var task = Substitute.For<IPipelineTask<object?>>();

            await PipelineDelegate.InvokeAllAsync(new[] {task}, null, CancellationToken.None);

            await task.Received(1).InvokeAsync(null, Arg.Any<PipelineDelegate<object?>>(), CancellationToken.None);
        }

        [Fact]
        public async Task InvokeReturnsCompletedTaskAfterLastTask()
        {
            var testDelegate = new PipelineDelegate<object>(Array.Empty<IPipelineTask<object>>());

            // No throw for empty task list
            await testDelegate.InvokeAsync(new object(), CancellationToken.None);
        }
    }
}