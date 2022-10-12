using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;

internal sealed class MultiplicationOperatorSyntaxNode : OperatorSyntaxNode
{
    public MultiplicationOperatorSyntaxNode(ISyntaxToken childSyntaxToken)
        : this((StarSyntaxToken) childSyntaxToken)
    {
    }
    
    public MultiplicationOperatorSyntaxNode(StarSyntaxToken childSyntaxToken)
    {
        ChildSyntaxToken = childSyntaxToken;
    }

    public override SyntaxToken ChildSyntaxToken { get; set; }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.MultiplicationOperatorSyntaxNode;

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