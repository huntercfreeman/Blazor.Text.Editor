using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class TypeScriptExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey TypeScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTypeScriptTextEditorModel(
            TextEditorFacts.TypeScript.TypeScriptTextEditorModelKey,
            nameof(TypeScriptExamples),
            DateTime.UtcNow,
            "TypeScript",
            TestData.TypeScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            TypeScriptTextEditorViewModelKey,
            TextEditorFacts.TypeScript.TypeScriptTextEditorModelKey);
        
        base.OnInitialized();
    }
}