using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;

internal abstract class OperatorSyntaxNode : SyntaxNode
{
    public abstract SyntaxToken ChildSyntaxToken { get; set; }
}