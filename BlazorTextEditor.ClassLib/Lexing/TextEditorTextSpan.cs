using BlazorTextEditor.ClassLib.TextEditor;

namespace BlazorTextEditor.ClassLib.Lexing;

public record TextEditorTextSpan(
    int StartingIndexInclusive, 
    int EndingIndexExclusive,
    byte DecorationByte);