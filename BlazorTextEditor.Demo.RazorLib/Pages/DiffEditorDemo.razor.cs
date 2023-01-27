using BlazorALaCarte.Shared.Dimensions;
using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class DiffEditorDemo : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorDiffKey DiffDemoDiffKey = TextEditorDiffKey.NewTextEditorDiffKey();
    
    private static readonly TextEditorViewModelKey DiffDemoBeforeViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    private static readonly TextEditorViewModelKey DiffDemoAfterViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.CSharp.DiffDemoBeforeModelKey,
            WellKnownModelKind.CSharp,
            "DiffDemoBefore",
            DateTime.UtcNow,
            "C#",
            TestData.Diff.BEFORE_TEXT);
        
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.CSharp.DiffDemoAfterModelKey,
            WellKnownModelKind.CSharp,
            "DiffDemoAfter",
            DateTime.UtcNow,
            "C#",
            TestData.Diff.AFTER_TEXT);
        
        TextEditorService.ViewModelRegister(
            DiffDemoBeforeViewModelKey,
            TextEditorFacts.CSharp.DiffDemoBeforeModelKey);
        
        TextEditorService.ViewModelRegister(
            DiffDemoAfterViewModelKey,
            TextEditorFacts.CSharp.DiffDemoAfterModelKey);
        
        TextEditorService.DiffRegister(
            DiffDemoDiffKey,
            DiffDemoBeforeViewModelKey,
            DiffDemoAfterViewModelKey);
        
        base.OnInitialized();
    }
    
    /// <summary>
    /// TODO: The values written in the calc() were thrown together quickly. This needs to be done thoroughly.
    /// </summary>
    private string GetTextEditorDiffDisplayCssStyleString(int rowCount)
    {
        return $"height: calc(100% - 6em - max(3em, 3ch) * {rowCount.ToCssValue()});";
    }
}