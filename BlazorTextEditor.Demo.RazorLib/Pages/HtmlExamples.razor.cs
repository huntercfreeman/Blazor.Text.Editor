using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class HtmlExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.Html.HtmlNineteenLinesTextEditorKey,
            TestData.Html.EXAMPLE_TEXT_19_LINES);
        
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.Html.HtmlCommentsTextEditorKey,
            TestData.Html.EXAMPLE_TEXT_COMMENTS);
        
        base.OnInitialized();
    }
}