using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class CssExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey CssTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCssTextEditor(
            TextEditorFacts.Css.CssTextEditorKey,
            nameof(CssExamples),
            DateTime.UtcNow,
            "CSS",
            TestData.Css.EXAMPLE_TEXT_21_LINES);

        TextEditorService.RegisterViewModel(
            CssTextEditorViewModelKey,
            TextEditorFacts.Css.CssTextEditorKey);
        
        base.OnInitialized();
    }
}