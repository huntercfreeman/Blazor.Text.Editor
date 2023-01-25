using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class FSharpDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorViewModelKey FSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.FSharp.FSharpTextEditorModelKey,
            WellKnownModelKind.FSharp,
            nameof(FSharpDemo),
            DateTime.UtcNow,
            "F#",
            TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        TextEditorService.RegisterViewModel(
            FSharpTextEditorViewModelKey,
            TextEditorFacts.FSharp.FSharpTextEditorModelKey);
        
        base.OnInitialized();
    }
}