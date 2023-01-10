using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class FSharpExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey FSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterFSharpTextEditorModel(
            TextEditorFacts.FSharp.FSharpTextEditorModelKey,
            nameof(FSharpExamples),
            DateTime.UtcNow,
            "F#",
            TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        TextEditorService.RegisterViewModel(
            FSharpTextEditorViewModelKey,
            TextEditorFacts.FSharp.FSharpTextEditorModelKey);
        
        base.OnInitialized();
    }
}