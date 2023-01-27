using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.Demo.RazorLib.TextEditorDemos;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Api;

public partial class TextEditorServiceApi : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorViewModelKey TextEditorServiceApiViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            TextEditorFacts.CSharp.TextEditorServiceApiModelKey,
            WellKnownModelKind.CSharp,
            nameof(TextEditorServiceApi),
            DateTime.UtcNow,
            "C#",
            TestData.CSharp.TEXT_EDITOR_SERVICE_API);
        
        TextEditorService.ViewModelRegister(
            TextEditorServiceApiViewModelKey,
            TextEditorFacts.CSharp.TextEditorServiceApiModelKey);
        
        base.OnInitialized();
    }
}