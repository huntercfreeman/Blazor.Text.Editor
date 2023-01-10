using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class HtmlExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey HtmlTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.Html.HtmlTextEditorModelKey,
            WellKnownModelKind.Html,
            nameof(HtmlExamples),
            DateTime.UtcNow,
            "HTML",
            TestData.Html.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            HtmlTextEditorViewModelKey,
            TextEditorFacts.Html.HtmlTextEditorModelKey);
        
        base.OnInitialized();
    }
}