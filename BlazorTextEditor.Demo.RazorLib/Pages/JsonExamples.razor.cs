using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class JsonExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterJsonTextEditor(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorKey,
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        // TextEditorService.RegisterJsonTextEditor(
        //     TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorKey,
        //     TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        //
        // TextEditorService.RegisterJsonTextEditor(
        //     TextEditorFacts.Json.JsonObjectWithArrayTextEditorKey,
        //     TestData.Json.EXAMPLE_TEXT_OBJECT_WITH_ARRAY);
        
        base.OnInitialized();
    }
}