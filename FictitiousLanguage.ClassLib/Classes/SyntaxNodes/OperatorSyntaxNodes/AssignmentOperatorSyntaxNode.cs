using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;

internal sealed class AssignmentOperatorSyntaxNode : OperatorSyntaxNode
{
    public AssignmentOperatorSyntaxNode(ISyntaxToken variableIdentifierSyntaxToken)
        : this((EqualsSyntaxToken)variableIdentifierSyntaxToken)
    {
    }
    
    public AssignmentOperatorSyntaxNode(EqualsSyntaxToken variableIdentifierSyntaxToken)
    {
        ChildSyntaxToken = variableIdentifierSyntaxToken;
    }

    public override SyntaxToken ChildSyntaxToken { get; set; }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.AssignmentOperatorSyntaxNode;

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