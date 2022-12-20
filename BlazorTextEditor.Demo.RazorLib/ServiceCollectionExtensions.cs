using BlazorTextEditor.Demo.ClassLib;
using BlazorTextEditor.RazorLib;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Demo.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorDemoRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddBlazorTextEditor()
            .AddBlazorTextEditorDemoClassLibServices();
    }
}