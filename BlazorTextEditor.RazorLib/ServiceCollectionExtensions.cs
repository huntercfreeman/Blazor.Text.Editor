using BlazorALaCarte.DialogNotification.Installation;
using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Installation;
using BlazorALaCarte.Shared.Services;
using BlazorALaCarte.Shared.Storage;
using BlazorALaCarte.TreeView.Installation;
using BlazorTextEditor.RazorLib.Autocomplete;
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
            .AddSharedServices()
            .AddDialogServices()
            .AddTreeViewServices()
            .AddTextEditorClassLibServices(
                serviceProvider =>
                    new JavaScriptInteropClipboardProvider(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                serviceProvider =>
                    new LocalStorageProvider(
                        serviceProvider.GetRequiredService<IJSRuntime>()),
                serviceProvider =>
                    new AutocompleteService(serviceProvider.GetRequiredService<IAutocompleteIndexer>()),
                serviceProvider =>
                    new AutocompleteIndexer(serviceProvider.GetRequiredService<ITextEditorService>()),
                configure);
    }

    private static IServiceCollection AddTextEditorClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        Func<IServiceProvider, IStorageProvider> storageProviderDefaultFactory,
        Func<IServiceProvider, IAutocompleteService> autocompleteServiceDefaultFactory,
        Func<IServiceProvider, IAutocompleteIndexer> autocompleteIndexerDefaultFactory,
        Action<TextEditorServiceOptions>? configure = null)
    {
        var textEditorOptions = new TextEditorServiceOptions();
        configure?.Invoke(textEditorOptions);

        var clipboardProviderFactory = textEditorOptions.ClipboardProviderFactory
                                       ?? clipboardProviderDefaultFactory;
        
        var storageProviderFactory = textEditorOptions.StorageProviderFactory
                                       ?? storageProviderDefaultFactory;
        
        var autocompleteServiceFactory = textEditorOptions.AutocompleteServiceFactory
                                                ?? autocompleteServiceDefaultFactory;
        
        var autocompleteIndexerFactory = textEditorOptions.AutocompleteIndexerFactory
                                                ?? autocompleteIndexerDefaultFactory;

        services
            .AddSingleton<ITextEditorServiceOptions, ImmutableTextEditorServiceOptions>(
                _ => new ImmutableTextEditorServiceOptions(textEditorOptions))
            .AddScoped(serviceProvider => clipboardProviderFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => storageProviderFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => autocompleteServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => autocompleteIndexerFactory.Invoke(serviceProvider))
            .AddScoped<IThemeService, ThemeService>()
            .AddScoped<ITextEditorService, TextEditorService>();

        if (textEditorOptions.InitializeFluxor)
        {
            services
                .AddFluxor(options => options
                    .ScanAssemblies(
                        typeof(ServiceCollectionExtensions).Assembly,
                        typeof(BlazorALaCarte.Shared.Installation.ServiceCollectionExtensions).Assembly,
                        typeof(BlazorALaCarte.DialogNotification.Installation.ServiceCollectionExtensions).Assembly,
                        typeof(BlazorALaCarte.TreeView.Installation.ServiceCollectionExtensions).Assembly));
        }

        return services;
    }
}