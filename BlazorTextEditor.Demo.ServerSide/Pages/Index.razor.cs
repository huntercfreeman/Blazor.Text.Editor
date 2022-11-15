using BlazorTextEditor.Demo.ServerSide.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.ServerSide.Pages;

public partial class Index
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        var textEditor = new TextEditorBase(
            string.Empty,
            null,
            null,
            null,
            TextEditorFacts.IndexTextEditorKey);
        
        TextEditorService.RegisterCustomTextEditor(
            textEditor);
        
        base.OnInitialized();
    }

    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}