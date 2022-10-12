namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class LiteralStringSyntaxToken : LiteralSyntaxToken
{
    public LiteralStringSyntaxToken(TextSpan textSpan, object value) 
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.LiteralStringToken;
}