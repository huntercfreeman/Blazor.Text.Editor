using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class RazorExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterRazorTextEditor(
            TextEditorFacts.Razor.RazorTextEditorKey,
            nameof(RazorExamples),
            TestData.Razor.EXAMPLE_TEXT);
        
        base.OnInitialized();
    }
}