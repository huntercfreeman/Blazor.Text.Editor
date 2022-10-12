namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class StarSyntaxToken : SyntaxToken
{
    public StarSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.StarToken;
}