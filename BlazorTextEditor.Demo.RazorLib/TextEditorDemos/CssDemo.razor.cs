using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class CssDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey CssTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.Css.CssTextEditorModelKey,
            WellKnownModelKind.Css,
            nameof(CssDemo),
            DateTime.UtcNow,
            "CSS",
            TestData.Css.EXAMPLE_TEXT_21_LINES);

        TextEditorService.ViewModelRegister(
            CssTextEditorViewModelKey,
            TextEditorFacts.Css.CssTextEditorModelKey);
        
        base.OnInitialized();
    }
}