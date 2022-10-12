using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;

internal sealed class SubtractionOperatorSyntaxNode : OperatorSyntaxNode
{
    public SubtractionOperatorSyntaxNode(ISyntaxToken childSyntaxToken)
        : this((MinusSyntaxToken)childSyntaxToken)
    {
    }
    
    public SubtractionOperatorSyntaxNode(MinusSyntaxToken childSyntaxToken)
    {
        ChildSyntaxToken = childSyntaxToken;
    }

    public override SyntaxToken ChildSyntaxToken { get; set; }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.SubtractionOperatorSyntaxNode;

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