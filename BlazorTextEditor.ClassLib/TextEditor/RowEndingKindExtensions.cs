namespace BlazorTextEditor.ClassLib.TextEditor;

public static class RowEndingKindExtensions
{
    /// <summary>
    /// In order to not override the ToString() method in a possibly unexpected way <see cref="AsCharacters"/> was made
    /// to convert a <see cref="RowEndingKind"/> to its character(s) representation.
    /// <br/><br/>
    /// Example: <see cref="RowEndingKind.NewLine"/> would return '\n'
    /// </summary>
    public static string AsCharacters(this RowEndingKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            RowEndingKind.CarriageReturn => "\r",
            RowEndingKind.NewLine => "\n",
            RowEndingKind.CarriageReturnNewLine => "\r\n",
            RowEndingKind.StartOfFile or RowEndingKind.EndOfFile => string.Empty,
            _ => throw new ApplicationException($"Unexpected {nameof(RowEndingKind)} of: {rowEndingKind}")
        };
    }
    
    public static string AsCharactersHtmlEscaped(this RowEndingKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            RowEndingKind.CarriageReturn => "\\r",
            RowEndingKind.NewLine => "\\n",
            RowEndingKind.CarriageReturnNewLine => "\\r\\n",
            RowEndingKind.StartOfFile => "SOF",
            RowEndingKind.EndOfFile => "EOF",
            _ => throw new ApplicationException($"Unexpected {nameof(RowEndingKind)} of: {rowEndingKind}")
        };
    }
}