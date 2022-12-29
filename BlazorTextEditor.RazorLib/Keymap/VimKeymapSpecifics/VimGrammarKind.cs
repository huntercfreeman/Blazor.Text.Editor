namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public enum VimGrammarKind
{
    // VimGrammarKind.Start removes the concept of null when lexing
    Start,
    Verb,
    Modifier,
    TextObject,
    Repeat
}