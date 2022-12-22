using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class JavaScriptExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey JavaScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterJavaScriptTextEditor(
            TextEditorFacts.JavaScript.JavaScriptTextEditorKey,
            nameof(JavaScriptExamples),
            TestData.JavaScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            JavaScriptTextEditorViewModelKey,
            TextEditorFacts.JavaScript.JavaScriptTextEditorKey);
        
        base.OnInitialized();
    }
}