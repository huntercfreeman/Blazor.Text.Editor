using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class CssExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey CssTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCssTextEditorModel(
            TextEditorFacts.Css.CssTextEditorModelKey,
            nameof(CssExamples),
            DateTime.UtcNow,
            "CSS",
            TestData.Css.EXAMPLE_TEXT_21_LINES);

        TextEditorService.RegisterViewModel(
            CssTextEditorViewModelKey,
            TextEditorFacts.Css.CssTextEditorModelKey);
        
        base.OnInitialized();
    }
}