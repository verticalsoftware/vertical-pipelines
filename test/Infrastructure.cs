using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vertical.Pipelines.Test
{
    public interface IService
    {
        void Call();
    }

    public class Request
    {
        public int Count { get; set; }
        public List<string> Strings { get; } = new();
    }

    public class RequestWithServiceProvider : Request, IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestWithServiceProvider(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        object? IServiceProvider.GetService(Type t) => _serviceProvider.GetService(t);
    }
}