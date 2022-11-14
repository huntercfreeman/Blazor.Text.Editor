using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditor(
        this IServiceCollection services,
        Action<TextEditorServiceOptions>? configure = null)
    {
        return services
            .AddTextEditorClassLibServices(
                serviceProvider =>
                    new JavaScriptInteropClipboardProvider(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                serviceProvider =>
                    new LocalStorageProvider(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                configure);
    }

    private static IServiceCollection AddTextEditorClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        Func<IServiceProvider, IStorageProvider> storageProviderDefaultFactory,
        Action<TextEditorServiceOptions>? configure = null)
    {
        var textEditorOptions = new TextEditorServiceOptions();
        configure?.Invoke(textEditorOptions);

        var clipboardProviderFactory = textEditorOptions.ClipboardProviderFactory
                                       ?? clipboardProviderDefaultFactory;
        
        var storageProviderFactory = textEditorOptions.StorageProviderFactory
                                       ?? storageProviderDefaultFactory;

        services
            .AddSingleton<ITextEditorServiceOptions, ImmutableTextEditorServiceOptions>(
                _ => new ImmutableTextEditorServiceOptions(textEditorOptions))
            .AddScoped(serviceProvider => clipboardProviderFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => storageProviderFactory.Invoke(serviceProvider))
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