using BlazorALaCarte.Shared.Clipboard;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.StorageCase;
using BlazorTextEditor.RazorLib.TextEditor;
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
    protected readonly TextEditorKey TextEditorKey = TextEditorKey.NewTextEditorKey();

    protected TextEditorBase TextEditor => TextEditorService.TextEditorStates.TextEditorList
        .First(x => x.Key == TextEditorKey);

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

        var textEditor = new TextEditorBase(
            nameof(BlazorTextEditorTestingBase),
            "UnitTests",
            string.Empty,
            null,
            null,
            null,
            TextEditorKey);
        
        TextEditorService.RegisterCustomTextEditor(textEditor);
    }
}