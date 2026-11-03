using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace HelpDesk.Application.Configuration;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ServiceFactory>(p => type => p.GetRequiredService(type));
        services.AddScoped<IMediator, Mediator>();
        foreach (var type in typeof(ApplicationServicesExtensions).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            foreach (var contract in type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                services.AddTransient(contract, type);
        services.AddScoped<AccessRules>();
        services.AddScoped<AuthService>();
        return services;
    }
}
