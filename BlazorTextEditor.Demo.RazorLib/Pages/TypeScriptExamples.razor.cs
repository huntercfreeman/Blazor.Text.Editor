using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class TypeScriptExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey TypeScriptTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTypeScriptTextEditor(
            TextEditorFacts.TypeScript.TypeScriptTextEditorKey,
            nameof(TypeScriptExamples),
            DateTime.UtcNow,
            "TypeScript",
            TestData.TypeScript.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            TypeScriptTextEditorViewModelKey,
            TextEditorFacts.TypeScript.TypeScriptTextEditorKey);
        
        base.OnInitialized();
    }
}