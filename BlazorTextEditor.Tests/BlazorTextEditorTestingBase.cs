using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.Storage;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorTextEditor.Tests;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class BlazorTextEditorTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ITextEditorService TextEditorService;
    protected readonly TextEditorModelKey TextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    protected readonly TextEditorViewModelKey TextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected TextEditorModel TextEditorModel => TextEditorService.Model
        .FindOrDefault(TextEditorModelKey)
            ?? throw new ApplicationException(
                $"{nameof(TextEditorService)}" +
                $".{nameof(TextEditorService.Model.FindOrDefault)}" +
                " returned null.");
    
    protected TextEditorViewModel TextEditorViewModel => TextEditorService.ViewModel
        .FindOrDefault(TextEditorViewModelKey)
            ?? throw new ApplicationException(
                $"{nameof(TextEditorService)}" +
                $".{nameof(TextEditorService.ViewModel.FindOrDefault)}" +
                " returned null.");

    public BlazorTextEditorTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        var shouldInitializeFluxor = false;
        
        services.AddBlazorTextEditor(inTextEditorOptions =>
        {
            var blazorCommonOptions = 
                (inTextEditorOptions.BlazorCommonOptions ?? new()) with
            {
                InitializeFluxor = shouldInitializeFluxor
            };

            var blazorCommonFactories = blazorCommonOptions.BlazorCommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };
            
            blazorCommonOptions = blazorCommonOptions with
            {
                BlazorCommonFactories = blazorCommonFactories
            };
            
            return inTextEditorOptions with
            {
                InitializeFluxor = shouldInitializeFluxor,
                CustomThemeRecords = BlazorTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = BlazorTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                BlazorCommonOptions = blazorCommonOptions 
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
            null,
            TextEditorModelKey);
        
        TextEditorService.Model.RegisterCustom(textEditor);
        
        TextEditorService.ViewModel.Register(
            TextEditorViewModelKey,
            TextEditorModelKey);
    }
}