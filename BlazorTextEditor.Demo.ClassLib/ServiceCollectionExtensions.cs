using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Demo.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorDemoClassLibServices(this IServiceCollection services)
    {
        return services;
    }
}