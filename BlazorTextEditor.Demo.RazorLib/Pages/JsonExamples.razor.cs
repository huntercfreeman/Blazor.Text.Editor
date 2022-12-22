using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class JsonExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private static readonly TextEditorGroupKey JsonTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();
    
    private static readonly TextEditorViewModelKey JsonTextEditorViewModelKeyOne = TextEditorViewModelKey.NewTextEditorViewModelKey();
    private static readonly TextEditorViewModelKey JsonTextEditorViewModelKeyTwo = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterGroup(JsonTextEditorGroupKey);
        
        TextEditorService.RegisterJsonTextEditor(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorKey,
            nameof(JsonExamples) + "_1",
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        TextEditorService.RegisterJsonTextEditor(
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorKey,
            nameof(JsonExamples) + "_2",
            TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        
        TextEditorService.RegisterViewModel(
            JsonTextEditorViewModelKeyOne,
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorKey);
        
        TextEditorService.RegisterViewModel(
            JsonTextEditorViewModelKeyTwo,
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorKey);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyOne);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyTwo);
        
        base.OnInitialized();
    }
}