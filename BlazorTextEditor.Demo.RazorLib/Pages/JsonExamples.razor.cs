using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
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
        
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorModelKey,
            WellKnownModelKind.Json,
            nameof(JsonExamples) + "_1",
            DateTime.UtcNow,
            "JSON",
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorModelKey,
            WellKnownModelKind.Json,
            nameof(JsonExamples) + "_2",
            DateTime.UtcNow,
            "JSON",
            TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        
        TextEditorService.RegisterViewModel(
            JsonTextEditorViewModelKeyOne,
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorModelKey);
        
        TextEditorService.RegisterViewModel(
            JsonTextEditorViewModelKeyTwo,
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorModelKey);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyOne);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyTwo);
        
        base.OnInitialized();
    }
}