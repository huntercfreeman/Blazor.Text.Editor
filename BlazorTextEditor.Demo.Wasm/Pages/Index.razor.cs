using BlazorTextEditor.Demo.Wasm.TestDataFolder;
using BlazorTextEditor.Demo.Wasm.TextEditor;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.Wasm.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditor(
            TextEditorFacts.CSharpTextEditor,
            TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        base.OnInitialized();
    }

    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}