using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace IDE.Core.Presentation.Infrastructure
{
    public class ServiceProviderHelper : IServiceProviderHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _services;

        public ServiceProviderHelper(IServiceProvider serviceProvider, IServiceCollection services)
        {
            _serviceProvider = serviceProvider;
            _services = services;
        }

        public IEnumerable<T> GetServices<T>() 
        {
           return from s in _services
                 where s.ServiceType.GetInterfaces().Contains(typeof(T))
                 select (T)_serviceProvider.GetService(s.ServiceType);
        }
    }
}
