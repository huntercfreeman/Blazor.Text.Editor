using BlazorTextEditor.RazorLib.Commands;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public interface ITextEditorKeymap
{
    public Func<KeyboardEventArgs, TextEditorCommand?> KeymapFunc { get; }
}