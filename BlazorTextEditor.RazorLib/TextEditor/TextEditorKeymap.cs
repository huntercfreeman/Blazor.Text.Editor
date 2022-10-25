using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TextEditor;

public class TextEditorKeymap : ITextEditorKeymap
{
    public TextEditorKeymap(
        Func<KeyboardEventArgs, TextEditorCommand> keymapFunc)
    {
        KeymapFunc = keymapFunc;
    }

    public Func<KeyboardEventArgs, TextEditorCommand> KeymapFunc { get; }
}