using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.Demo.RazorLib.TextEditorDemos;
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
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.CSharp.DiffDemoBeforeModelKey,
            WellKnownModelKind.CSharp,
            "DiffDemoBefore",
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.DIFF_DEMO_BEFORE_TEXT);
        
        TextEditorService.RegisterTemplatedTextEditorModel(
            TextEditorFacts.CSharp.DiffDemoAfterModelKey,
            WellKnownModelKind.CSharp,
            "DiffDemoAfter",
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.DIFF_DEMO_AFTER_TEXT);
        
        TextEditorService.RegisterViewModel(
            DiffDemoBeforeViewModelKey,
            TextEditorFacts.CSharp.DiffDemoBeforeModelKey);
        
        TextEditorService.RegisterViewModel(
            DiffDemoAfterViewModelKey,
            TextEditorFacts.CSharp.DiffDemoAfterModelKey);
        
        TextEditorService.RegisterDiff(
            DiffDemoDiffKey,
            DiffDemoBeforeViewModelKey,
            DiffDemoAfterViewModelKey);
        
        base.OnInitialized();
    }
}