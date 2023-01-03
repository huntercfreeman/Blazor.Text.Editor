using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap.Vim;

public record VimGrammarToken(
    VimGrammarKind VimGrammarKind,
    KeyboardEventArgs KeyboardEventArgs);