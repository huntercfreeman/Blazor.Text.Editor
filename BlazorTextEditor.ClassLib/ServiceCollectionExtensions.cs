using BlazorTextEditor.ClassLib.Clipboard;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextEditorClassLibServices(
        this IServiceCollection services,
        Func<IServiceProvider, IClipboardProvider> clipboardProviderDefaultFactory,
        Action<TextEditorOptions>? configure = null)
    {
        var textEditorOptions = new TextEditorOptions();
        configure?.Invoke(textEditorOptions);

        var clipboardProviderFactory = textEditorOptions.ClipboardProviderFactory
                                       ?? clipboardProviderDefaultFactory;
        
        if (textEditorOptions.InitializeFluxor)
        {
            services
                .AddSingleton<ITextEditorOptions, ImmutableTextEditorOptions>(
                    _ => new ImmutableTextEditorOptions(textEditorOptions))
                .AddScoped<IClipboardProvider>(serviceProvider => clipboardProviderFactory.Invoke(serviceProvider))
                .AddScoped<ITextEditorService, TextEditorService>()
                .AddFluxor(options => options
                    .ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly));
        }

        return services;
    }
}