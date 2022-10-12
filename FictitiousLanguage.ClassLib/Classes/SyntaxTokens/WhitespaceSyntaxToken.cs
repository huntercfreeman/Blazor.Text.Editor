namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class WhitespaceSyntaxToken : SyntaxToken
{
    public WhitespaceSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.WhitespaceToken;
}