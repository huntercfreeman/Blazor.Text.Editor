namespace BlazorTextEditor.RazorLib.Lexing;

public record TextEditorTextSpan(
    int StartingIndexInclusive,
    int EndingIndexExclusive,
    byte DecorationByte);