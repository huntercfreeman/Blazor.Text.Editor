using BlazorTextEditor.ClassLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextEditorRazorLibServices(
        this IServiceCollection services,
        Action<TextEditorOptions>? configure = null)
    {
        return services
            .AddTextEditorClassLibServices(
                (serviceProvider) => 
                    new ClipboardProviderDefault(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                configure);
    }
}