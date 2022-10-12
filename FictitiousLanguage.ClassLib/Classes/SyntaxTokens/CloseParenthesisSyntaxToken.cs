namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class CloseParenthesisSyntaxToken : SyntaxToken
{
    public CloseParenthesisSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.CloseParenthesisToken;
}