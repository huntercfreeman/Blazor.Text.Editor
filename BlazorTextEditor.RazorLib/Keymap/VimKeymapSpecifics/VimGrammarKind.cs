namespace BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;

public enum VimGrammarKind
{
    // VimGrammarKind.Start removes the concept of null when lexing
    Start,
    Command,
    Expansion,
    Motion,
    Repeat
}