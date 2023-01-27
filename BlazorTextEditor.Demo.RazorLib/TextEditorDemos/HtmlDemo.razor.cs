using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class HtmlDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey HtmlTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.Html.HtmlTextEditorModelKey,
            WellKnownModelKind.Html,
            nameof(HtmlDemo),
            DateTime.UtcNow,
            "HTML",
            TestData.Html.EXAMPLE_TEXT);
        
        TextEditorService.ViewModelRegister(
            HtmlTextEditorViewModelKey,
            TextEditorFacts.Html.HtmlTextEditorModelKey);
        
        base.OnInitialized();
    }
}