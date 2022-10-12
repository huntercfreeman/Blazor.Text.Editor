using FictitiousLanguage.ClassLib.Classes;

namespace FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

public interface ISyntaxToken
{
    public SyntaxTokenKind Kind { get; }
    public TextSpan TextSpan { get; }
    public object Value { get; }
}