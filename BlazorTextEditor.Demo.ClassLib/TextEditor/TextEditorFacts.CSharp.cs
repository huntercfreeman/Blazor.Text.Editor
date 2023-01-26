using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.Demo.ClassLib.TextEditor;

public static partial class TextEditorFacts
{
    public static class CSharp
    {
        public static readonly TextEditorModelKey CSharpTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey TextEditorServiceApiModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey MyClassTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    }
}