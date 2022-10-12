namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class MinusSyntaxToken : SyntaxToken
{
    public MinusSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.MinusToken;
}