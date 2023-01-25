using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class JavaScriptDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey JavaScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.JavaScript.JavaScriptTextEditorModelKey,
            WellKnownModelKind.JavaScript,
            nameof(JavaScriptDemo),
            DateTime.UtcNow,
            "JavaScript",
            TestData.JavaScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            JavaScriptTextEditorViewModelKey,
            TextEditorFacts.JavaScript.JavaScriptTextEditorModelKey);
        
        base.OnInitialized();
    }
}