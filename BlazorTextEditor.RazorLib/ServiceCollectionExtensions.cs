using BlazorTextEditor.RazorLib.Clipboard;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextEditorRazorLibServices(
        this IServiceCollection services,
        Action<TextEditorServiceOptions>? configure = null)
    {
        return services
            .AddTextEditorClassLibServices(
                serviceProvider =>
                    new ClipboardProviderDefault(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                configure);
    }

    public static IServiceCollection AddTextEditorClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        Action<TextEditorServiceOptions>? configure = null)
    {
        var textEditorOptions = new TextEditorServiceOptions();
        configure?.Invoke(textEditorOptions);

        var clipboardProviderFactory = textEditorOptions.ClipboardProviderFactory
                                       ?? clipboardProviderDefaultFactory;

        services
            .AddSingleton<ITextEditorServiceOptions, ImmutableTextEditorServiceOptions>(
                _ => new ImmutableTextEditorServiceOptions(textEditorOptions))
            .AddScoped(serviceProvider => clipboardProviderFactory.Invoke(serviceProvider))
            .AddScoped<IThemeService, ThemeService>()
            .AddScoped<ITextEditorService, TextEditorService>();

        if (textEditorOptions.InitializeFluxor)
        {
            services
                .AddFluxor(options => options
                    .ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly));
        }

        return services;
    }
}