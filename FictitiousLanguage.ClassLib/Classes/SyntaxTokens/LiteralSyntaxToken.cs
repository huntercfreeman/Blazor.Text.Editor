namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal abstract class LiteralSyntaxToken : SyntaxToken
{
    protected LiteralSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }
}