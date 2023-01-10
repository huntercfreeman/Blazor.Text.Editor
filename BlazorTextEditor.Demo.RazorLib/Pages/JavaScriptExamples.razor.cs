using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class JavaScriptExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey JavaScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterJavaScriptTextEditorModel(
            TextEditorFacts.JavaScript.JavaScriptTextEditorModelKey,
            nameof(JavaScriptExamples),
            DateTime.UtcNow,
            "JavaScript",
            TestData.JavaScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            JavaScriptTextEditorViewModelKey,
            TextEditorFacts.JavaScript.JavaScriptTextEditorModelKey);
        
        base.OnInitialized();
    }
}