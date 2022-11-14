using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
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
    protected readonly ServiceProvider _serviceProvider;
    protected readonly ITextEditorService _textEditorService;
    protected readonly TextEditorKey _textEditorKey = TextEditorKey.NewTextEditorKey();

    protected TextEditorBase _textEditor => _textEditorService.TextEditorStates.TextEditorList
        .First(x => x.Key == _textEditorKey);

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
                    typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions)
                        .Assembly));

        _serviceProvider = services.BuildServiceProvider();

        var store = _serviceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();

        _textEditorService = _serviceProvider
            .GetRequiredService<ITextEditorService>();

        var textEditor = new TextEditorBase(
            string.Empty, 
            null,
            null,
            null,
            _textEditorKey);
        
        _textEditorService.RegisterTextEditor(textEditor);
    }
}