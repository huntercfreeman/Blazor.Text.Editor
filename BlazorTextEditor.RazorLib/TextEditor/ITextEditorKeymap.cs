using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TextEditor;

public interface ITextEditorKeymap
{
    public Func<KeyboardEventArgs, TextEditorCommand> KeymapFunc { get; }
}