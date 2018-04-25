using MessagesTransferApi.Logic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagesTransferApi.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddTokenGeneratorService(this IServiceCollection services)
        {
            services.AddTransient<ITokenGeneratorService, GuidTokenGeneratorService>();
        }

        public static void AddConnectorSenderService(this IServiceCollection services)
        {
            services.AddTransient<IConnectorSenderService, DirectConnecterSenderService>();
        }

        public static void AddAggregatorSenderService(this IServiceCollection services)
        {
            services.AddTransient<IAggregatorSenderService, DirectAggregatorSenderService>();
        }
    }
}
