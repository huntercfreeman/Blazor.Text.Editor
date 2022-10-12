namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class PlusSyntaxToken : SyntaxToken
{
    public PlusSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.PlusToken;
}