using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PlayCodeApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPlayCodeApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}