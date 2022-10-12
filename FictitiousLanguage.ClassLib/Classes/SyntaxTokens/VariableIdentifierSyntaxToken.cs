namespace FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

internal sealed class VariableIdentifierSyntaxToken : SyntaxToken
{
    public VariableIdentifierSyntaxToken(TextSpan textSpan, object value)
        : base(textSpan, value)
    {
    }

    public override SyntaxTokenKind Kind => SyntaxTokenKind.VariableIdentifierToken;
}