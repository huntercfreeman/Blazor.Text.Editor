namespace BlazorTextEditor.RazorLib.Lexing;

public record TextEditorTextSpan(
    int StartingIndexInclusive,
    int EndingIndexExclusive,
    byte DecorationByte)
{
    public string GetText(string text)
    {
        return text.Substring(
            StartingIndexInclusive,
            EndingIndexExclusive - StartingIndexInclusive);
    }
}