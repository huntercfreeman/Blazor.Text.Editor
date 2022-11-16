using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.Demo.ServerSide.TextEditor;

public static class TextEditorFacts
{
    public static readonly TextEditorKey TypeScriptTextEditor = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorKey JavaScriptTextEditor = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorKey RazorTextEditor = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorKey HtmlTextEditor = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorKey CSharpTextEditor = TextEditorKey.NewTextEditorKey();
    public static readonly TextEditorKey PlainTextEditor = TextEditorKey.NewTextEditorKey();
}