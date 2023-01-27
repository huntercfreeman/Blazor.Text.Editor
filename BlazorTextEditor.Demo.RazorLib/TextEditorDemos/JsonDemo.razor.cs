using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class JsonDemo : ComponentBase
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
        TextEditorService.GroupRegister(JsonTextEditorGroupKey);
        
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorModelKey,
            WellKnownModelKind.Json,
            nameof(JsonDemo) + "_1",
            DateTime.UtcNow,
            "JSON",
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorModelKey,
            WellKnownModelKind.Json,
            nameof(JsonDemo) + "_2",
            DateTime.UtcNow,
            "JSON",
            TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        
        TextEditorService.ViewModelRegister(
            JsonTextEditorViewModelKeyOne,
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorModelKey);
        
        TextEditorService.ViewModelRegister(
            JsonTextEditorViewModelKeyTwo,
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorModelKey);
        
        TextEditorService.GroupAddViewModel(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyOne);
        
        TextEditorService.GroupAddViewModel(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyTwo);
        
        base.OnInitialized();
    }
}