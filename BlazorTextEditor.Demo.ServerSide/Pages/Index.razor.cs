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
        TextEditorService.RegisterHtmlTextEditor(
            TextEditorFacts.HtmlTextEditorKey,
            TestData.Html.EXAMPLE_TEXT_19_LINES);
        
        // TextEditorService.RegisterCSharpTextEditor(
        //     TextEditorFacts.CSharpTextEditorKey,
        //     string.Empty);
        
        // TextEditorService.RegisterCSharpTextEditor(
        //     TextEditorFacts.CSharpTextEditorKey,
        //     TestData.CSharp.EXAMPLE_TEXT_173_LINES);
        
        // TextEditorService.RegisterCssTextEditor(
        //     TextEditorFacts.CssTextEditorKey,
        //     TestData.Css.EXAMPLE_TEXT_21_LINES);
        
        // TextEditorService.RegisterFSharpTextEditor(
        //     TextEditorFacts.FSharpTextEditorKey,
        //     TestData.FSharp.EXAMPLE_TEXT_21_LINES);
        
        // TextEditorService.RegisterJavaScriptTextEditor(
        //     TextEditorFacts.JavaScriptTextEditorKey,
        //     TestData.JavaScript.EXAMPLE_TEXT_28_LINES);
        //
        // TextEditorService.RegisterPlainTextEditor(
        //     TextEditorFacts.PlainTextEditorKey,
        //     TestData.Plain.EXAMPLE_TEXT_25_LINES);
        //
        // TextEditorService.RegisterRazorTextEditor(
        //     TextEditorFacts.RazorTextEditorKey,
        //     TestData.Razor.EXAMPLE_TEXT_20_LINES);
        //
        // TextEditorService.RegisterTypeScriptTextEditor(
        //     TextEditorFacts.TypeScriptTextEditorKey,
        //     TestData.TypeScript.EXAMPLE_TEXT_28_LINES);
        
        base.OnInitialized();
    }

    private void OpenSettingsDialogOnClick()
    {
        TextEditorService.ShowSettingsDialog();
    }
}