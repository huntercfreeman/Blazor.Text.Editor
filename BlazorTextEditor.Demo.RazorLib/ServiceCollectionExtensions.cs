using BlazorTextEditor.Demo.ClassLib;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Demo.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorDemoRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddBlazorTextEditor(options => options.InitialTheme = ThemeFacts.VisualStudioLightClone)
            .AddBlazorTextEditorDemoClassLibServices();
    }
}