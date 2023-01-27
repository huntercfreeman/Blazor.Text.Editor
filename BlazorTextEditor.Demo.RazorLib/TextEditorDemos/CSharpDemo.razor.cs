using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.TextEditorDemos;

public partial class CSharpDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey CSharpTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.CSharp.CSharpTextEditorModelKey,
            WellKnownModelKind.CSharp,
            nameof(CSharpDemo),
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        TextEditorService.ViewModelRegister(
            CSharpTextEditorViewModelKey,
            TextEditorFacts.CSharp.CSharpTextEditorModelKey);
        
        base.OnInitialized();
    }
}