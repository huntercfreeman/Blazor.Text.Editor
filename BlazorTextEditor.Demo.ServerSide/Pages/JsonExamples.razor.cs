using BlazorTextEditor.Demo.ServerSide.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.Tests.TestDataFolder;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.ServerSide.Pages;

public partial class JsonExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditor(
            TextEditorFacts.JsonTextEditorKey,
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        TextEditorService.RegisterCSharpTextEditor(
            TextEditorFacts.JsonTextEditorKey,
            TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        
        TextEditorService.RegisterCSharpTextEditor(
            TextEditorFacts.JsonTextEditorKey,
            TestData.Json.EXAMPLE_TEXT_OBJECT_WITH_ARRAY);
        
        base.OnInitialized();
    }
}