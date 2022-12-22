using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class FSharpExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey FSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterFSharpTextEditor(
            TextEditorFacts.FSharp.FSharpTextEditorKey,
            nameof(FSharpExamples),
            TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        TextEditorService.RegisterViewModel(
            FSharpTextEditorViewModelKey,
            TextEditorFacts.FSharp.FSharpTextEditorKey);
        
        base.OnInitialized();
    }
}