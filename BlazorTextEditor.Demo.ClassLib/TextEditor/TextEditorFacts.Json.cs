using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.Demo.ClassLib.TextEditor;

public static partial class TextEditorFacts
{
    public static class Json
    {
        public static readonly TextEditorModelKey JsonLaunchSettingsTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey JsonArrayAsTopLevelTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey JsonObjectWithArrayTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey JsonWithCommentsTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
        public static readonly TextEditorModelKey JsonAdhocTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
    }
}
