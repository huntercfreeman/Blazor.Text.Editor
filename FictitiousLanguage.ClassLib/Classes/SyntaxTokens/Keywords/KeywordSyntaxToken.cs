namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens.Keywords;

internal sealed class KeywordSyntaxToken : SyntaxToken
{
    public KeywordSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.KeywordToken;
}