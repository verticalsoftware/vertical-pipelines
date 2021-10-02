﻿using System;
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
            var services = Substitute.For<IServiceProvider>();
            var dataService = Substitute.For<IDataService>();
            dataService.GetData("id").Returns("the-data");
            services.GetService(typeof(IDataService)).Returns(dataService);
            var builder = new PipelineBuilder<TestContext>();

            builder.UseMiddleware(typeof(MiddlewareA), services.GetService(typeof(IDataService)));
            builder.UseMiddleware(typeof(MiddlewareB));

            var pipeline = builder.Build();

            await pipeline(context, services);

            context.Count.ShouldBe(2);
            context.Data.ShouldBe("the-data");
        }
    }
}