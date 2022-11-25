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
            string.Empty);
        
        // TextEditorService.RegisterCSharpTextEditor(
        //     TextEditorFacts.CSharpTextEditor,
        //     TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        TextEditorService.RegisterCssTextEditor(
            TextEditorFacts.CssTextEditor,
            TestData.Css.EXAMPLE_TEXT_21_LINES);
        
        TextEditorService.RegisterFSharpTextEditor(
            TextEditorFacts.FSharpTextEditor,
            TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.HtmlTextEditor,
            TestData.Html.EXAMPLE_TEXT_19_LINES);
        
        TextEditorService.RegisterJavaScriptTextEditor(
            TextEditorFacts.JavaScriptTextEditor,
            TestData.JavaScript.EXAMPLE_TEXT_28_LINES);
        
        TextEditorService.RegisterPlainTextEditor(
            TextEditorFacts.PlainTextEditor,
            TestData.Plain.EXAMPLE_TEXT_25_LINES);
        
        TextEditorService.RegisterRazorTextEditor(
            TextEditorFacts.RazorTextEditor,
            TestData.Razor.EXAMPLE_TEXT_20_LINES);
        
        TextEditorService.RegisterTypeScriptTextEditor(
            TextEditorFacts.TypeScriptTextEditor,
            TestData.TypeScript.EXAMPLE_TEXT_28_LINES);
        
        base.OnInitialized();
    }

    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}