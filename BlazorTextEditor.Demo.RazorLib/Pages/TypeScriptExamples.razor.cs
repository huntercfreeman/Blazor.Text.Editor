using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class TypeScriptExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterTypeScriptTextEditor(
            TextEditorFacts.TypeScript.TypeScriptTextEditorKey,
            nameof(TypeScriptExamples),
            TestData.TypeScript.EXAMPLE_TEXT);
        
        base.OnInitialized();
    }
}