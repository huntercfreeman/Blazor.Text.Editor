namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class LiteralNumericSyntaxToken : LiteralSyntaxToken
{
    public LiteralNumericSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.LiteralNumericToken;
}