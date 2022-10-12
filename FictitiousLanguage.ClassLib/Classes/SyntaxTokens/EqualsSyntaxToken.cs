namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class EqualsSyntaxToken : SyntaxToken
{
    public EqualsSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.EqualsToken;
}