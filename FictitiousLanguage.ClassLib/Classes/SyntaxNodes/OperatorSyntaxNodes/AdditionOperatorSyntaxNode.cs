using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;

internal sealed class AdditionOperatorSyntaxNode : OperatorSyntaxNode
{
    public AdditionOperatorSyntaxNode(ISyntaxToken childSyntaxToken)
        : this((PlusSyntaxToken)childSyntaxToken)
    {
    }
    
    public AdditionOperatorSyntaxNode(PlusSyntaxToken childSyntaxToken)
    {
        ChildSyntaxToken = childSyntaxToken;
    }

    public override SyntaxToken ChildSyntaxToken { get; set; }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.AdditionOperatorSyntaxNode;

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