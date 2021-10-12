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

    public class RequestWithServiceProvider : Request, IApplicationServices
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestWithServiceProvider(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        IServiceProvider IApplicationServices.ApplicationServices => _serviceProvider;
    }
}