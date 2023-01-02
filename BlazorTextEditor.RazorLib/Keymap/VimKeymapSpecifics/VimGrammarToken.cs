using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public record VimGrammarToken(
    VimGrammarKind VimGrammarKind,
    KeyboardEventArgs KeyboardEventArgs);