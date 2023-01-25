using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class TypeScriptDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey TypeScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.TypeScript.TypeScriptTextEditorModelKey,
            WellKnownModelKind.TypeScript,
            nameof(TypeScriptDemo),
            DateTime.UtcNow,
            "TypeScript",
            TestData.TypeScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            TypeScriptTextEditorViewModelKey,
            TextEditorFacts.TypeScript.TypeScriptTextEditorModelKey);
        
        base.OnInitialized();
    }
}