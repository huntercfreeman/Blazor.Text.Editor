using BlazorTextEditor.Demo.ServerSide.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.Tests.TestDataFolder;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.ServerSide.Pages;

public partial class Index
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditor(
            TextEditorFacts.CSharpTextEditor,
            TestData.C_SHARP_EXAMPLE_TEXT);
        
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.HtmlTextEditor,
            TestData.HTML_EXAMPLE_TEXT);
        
        TextEditorService.RegisterRazorTextEditor(
            TextEditorFacts.RazorTextEditor,
            TestData.RAZOR_EXAMPLE_TEXT);
        
        TextEditorService.RegisterJavaScriptTextEditor(
            TextEditorFacts.JavaScriptTextEditor,
            TestData.JAVA_SCRIPT_EXAMPLE_TEXT);
        
        TextEditorService.RegisterTypeScriptTextEditor(
            TextEditorFacts.TypeScriptTextEditor,
            TestData.TYPE_SCRIPT_EXAMPLE_TEXT);
        
        TextEditorService.RegisterPlainTextEditor(
            TextEditorFacts.PlainTextEditor,
            TestData.PLAIN_EXAMPLE_TEXT);
        
        base.OnInitialized();
    }

    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}