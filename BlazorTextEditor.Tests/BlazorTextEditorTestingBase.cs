using BlazorCommon.RazorLib;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.Storage;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
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
    protected readonly TextEditorModelKey TextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();

    protected TextEditorModel TextEditorModel => TextEditorService
        .ModelFindOrDefault(TextEditorModelKey)
            ?? throw new ApplicationException(
                $"{nameof(TextEditorService)}" +
                $".{nameof(TextEditorService.ModelFindOrDefault)}" +
                " returned null.");

    public BlazorTextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddBlazorCommonServices(options =>
        {
            var inBlazorCommonFactories = options.BlazorCommonFactories;
            
            return options with
            {
                InitializeFluxor = false,
                BlazorCommonFactories = inBlazorCommonFactories with
                {
                    ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                    StorageServiceFactory = _ => new DoNothingStorageService(true)
                }
            };
        });
        
        services.AddFluxor(options => options
            .ScanAssemblies(
                typeof(BlazorCommon.RazorLib.ServiceCollectionExtensions).Assembly,
                typeof(BlazorTextEditor.RazorLib.ServiceCollectionExtensions).Assembly));

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
        
        TextEditorService.ModelRegisterCustomModel(textEditor);
    }
}