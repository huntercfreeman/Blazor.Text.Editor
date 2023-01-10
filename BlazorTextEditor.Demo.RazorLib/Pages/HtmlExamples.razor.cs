using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class HtmlExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey HtmlTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.Html.HtmlTextEditorKey,
            nameof(HtmlExamples),
            DateTime.UtcNow,
            "HTML",
            TestData.Html.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            HtmlTextEditorViewModelKey,
            TextEditorFacts.Html.HtmlTextEditorKey);
        
        base.OnInitialized();
    }
}