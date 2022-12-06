using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class FSharpExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterFSharpTextEditor(
            TextEditorFacts.FSharp.FSharpTextEditorKey,
            TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        base.OnInitialized();
    }
}