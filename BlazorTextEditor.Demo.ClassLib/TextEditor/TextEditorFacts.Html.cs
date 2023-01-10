using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.Demo.ClassLib.TextEditor;

public static partial class TextEditorFacts
{
    public static class Html
    {
        public static readonly TextEditorModelKey HtmlTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey HtmlCommentsTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    }
}