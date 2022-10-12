using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal abstract class SyntaxToken : ISyntaxToken
{
    public SyntaxToken(TextSpan textSpan, object value)
    {
        Value = value;
        TextSpan = textSpan;
    }

    public abstract SyntaxTokenKind Kind { get; }
    public TextSpan TextSpan { get; }
    public object Value { get; }
}