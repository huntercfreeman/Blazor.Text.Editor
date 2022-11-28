using BlazorTextEditor.Demo.ClassLib;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Demo.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorDemoRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddBlazorTextEditorDemoClassLibServices();
    }
}