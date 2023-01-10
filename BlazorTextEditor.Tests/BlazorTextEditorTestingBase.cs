using BlazorALaCarte.Shared.Clipboard;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.Tests;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class BlazorTextEditorTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly TextEditorModelKey TextEditorModelKey = TextEditorModelKey.NewTextEditorKey();

    protected TextEditorModel TextEditorModel => TextEditorService
        .GetTextEditorModelOrDefault(TextEditorModelKey)
            ?? throw new ApplicationException(
                $"{nameof(TextEditorService)}" +
                $".{nameof(TextEditorService.GetTextEditorModelOrDefault)}" +
                " returned null.");

    public BlazorTextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddBlazorTextEditor(options =>
        {
            options.InitializeFluxor = false;
            
            options.ClipboardProviderFactory = _ => 
                new InMemoryClipboardProvider();
            
            options.StorageProviderFactory = _ =>
                new DoNothingStorageProvider();
        });
        
        services
            .AddFluxor(options => options
                .ScanAssemblies(
                    typeof(RazorLib.ServiceCollectionExtensions)
                        .Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        TextEditorService = ServiceProvider
            .GetRequiredService<ITextEditorService>();

        var textEditor = new TextEditorModel(
            nameof(BlazorTextEditorTestingBase),
            DateTime.UtcNow,
            "UnitTests",
            string.Empty,
            null,
            null,
            null,
            TextEditorModelKey);
        
        TextEditorService.RegisterCustomTextEditor(textEditor);
    }
}