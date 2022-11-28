using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.Demo.ClassLib.TextEditor;

public static partial class TextEditorFacts
{
    public static class Json
    {
        public static readonly TextEditorKey JsonLaunchSettingsTextEditorKey = TextEditorKey.NewTextEditorKey();
        public static readonly TextEditorKey JsonArrayAsTopLevelTextEditorKey = TextEditorKey.NewTextEditorKey();
        public static readonly TextEditorKey JsonObjectWithArrayTextEditorKey = TextEditorKey.NewTextEditorKey();
    }
}
