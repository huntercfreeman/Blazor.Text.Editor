using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal sealed class LiteralNumericExpressionSyntaxNode : NumericExpressionSyntaxNode
{
    public LiteralNumericExpressionSyntaxNode(ISyntaxToken syntaxToken)
        : this((LiteralNumericSyntaxToken)syntaxToken)
    {
    }
    
    public LiteralNumericExpressionSyntaxNode(LiteralNumericSyntaxToken literalNumericSyntaxToken)
    {
        LiteralNumericSyntaxToken = literalNumericSyntaxToken;
    }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.LiteralNumericExpressionSyntaxNode;
    public LiteralNumericSyntaxToken LiteralNumericSyntaxToken { get; set; }

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[]
            {
            });
        }
    }
}