using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class CssExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCssTextEditor(
            TextEditorFacts.Css.CssTextEditorKey,
            nameof(CssExamples),
            TestData.Css.EXAMPLE_TEXT_21_LINES);
        
        base.OnInitialized();
    }
}