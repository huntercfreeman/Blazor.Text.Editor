using BlazorCommon.RazorLib;
using BlazorCommon.RazorLib.Theme;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditor(
        this IServiceCollection services,
        Func<BlazorTextEditorOptions, BlazorTextEditorOptions>? configure = null)
    {
        var textEditorOptions = new BlazorTextEditorOptions();
        configure?.Invoke(textEditorOptions);
        
        if (textEditorOptions.BlazorCommonOptions is not null)
        {
            services.AddBlazorCommonServices(options => 
                textEditorOptions.BlazorCommonOptions);
        }

        services
            .AddSingleton(textEditorOptions)
            .AddScoped(serviceProvider => textEditorOptions.AutocompleteServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => textEditorOptions.AutocompleteIndexerFactory.Invoke(serviceProvider))
            .AddScoped<IThemeRecordsCollectionService, ThemeRecordsCollectionService>()
            .AddScoped<ITextEditorService, TextEditorService>();

        if (textEditorOptions.InitializeFluxor)
        {
            services
                .AddFluxor(options => options
                    .ScanAssemblies(
                        typeof(ServiceCollectionExtensions).Assembly,
                        typeof(BlazorCommon.RazorLib.ServiceCollectionExtensions).Assembly));
        }
        
        return services;
    }
}