using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class CSharpExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey CSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditorModel(
            TextEditorFacts.CSharp.CSharpTextEditorModelKey,
            nameof(CSharpExamples),
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        TextEditorService.RegisterViewModel(
            CSharpTextEditorViewModelKey,
            TextEditorFacts.CSharp.CSharpTextEditorModelKey);
        
        base.OnInitialized();
    }
}