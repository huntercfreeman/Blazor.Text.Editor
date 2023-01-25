using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class RazorDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey RazorTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.Razor.RazorTextEditorModelKey,
            WellKnownModelKind.Razor,
            nameof(RazorDemo),
            DateTime.UtcNow,
            "Razor",
            TestData.Razor.EXAMPLE_TEXT);
        
        TextEditorService.RegisterViewModel(
            RazorTextEditorViewModelKey,
            TextEditorFacts.Razor.RazorTextEditorModelKey);
        
        base.OnInitialized();
    }
}