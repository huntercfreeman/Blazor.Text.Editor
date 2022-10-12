namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class SlashSyntaxToken : SyntaxToken
{
    public SlashSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.SlashToken;
}