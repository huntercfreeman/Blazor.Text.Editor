using BlazorCommon.RazorLib.Theme;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditor(
        this IServiceCollection services,
        Action<TextEditorServiceOptions>? configure = null)
    {
        var textEditorOptions = new TextEditorServiceOptions();
        configure?.Invoke(textEditorOptions);

        services
            .AddSingleton<ITextEditorServiceOptions, ImmutableTextEditorServiceOptions>(
                _ => new ImmutableTextEditorServiceOptions(textEditorOptions))
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