namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class StatementDelimiterSyntaxToken : SyntaxToken
{
    public StatementDelimiterSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.StatementDelimiterToken;
}