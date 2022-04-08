using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nordavind.Infrastructure.AspNetCore.Extensions;

namespace Cryptaur.Burse.Contract
{
    public static class BurseApiExtensions
    {
        public static IServiceCollection AddBurseApi(this IServiceCollection service, IConfiguration configuration)
        {
            configuration.Register<BurseApiSettings>(service);
            service.AddSingleton<BurseApi>();
            return service;
        }
    }
}