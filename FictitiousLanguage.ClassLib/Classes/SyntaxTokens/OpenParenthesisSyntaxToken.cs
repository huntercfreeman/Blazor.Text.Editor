namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class OpenParenthesisSyntaxToken : SyntaxToken
{
    public OpenParenthesisSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.OpenParenthesisToken;
}