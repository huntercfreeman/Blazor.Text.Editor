namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public record VimGrammarToken(
    VimGrammarKind VimGrammarKind,
    string TextValue);