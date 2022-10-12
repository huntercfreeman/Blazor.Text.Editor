using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal class VariableBoundNumericExpressionSyntaxNode : NumericExpressionSyntaxNode
{
    public VariableBoundNumericExpressionSyntaxNode(ISyntaxToken variableIdentifierSyntaxToken)
        : this((VariableIdentifierSyntaxToken)variableIdentifierSyntaxToken)
    {
    }
    
    public VariableBoundNumericExpressionSyntaxNode(VariableIdentifierSyntaxToken variableIdentifierSyntaxToken)
    {
        VariableIdentifierSyntaxToken = variableIdentifierSyntaxToken;
    }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.VariableBoundNumericExpressionSyntaxNode;
    public VariableIdentifierSyntaxToken VariableIdentifierSyntaxToken { get; set; }
    public NumericExpressionSyntaxNode? InnerNumericExpression { get; set; }

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[]
            {
                InnerNumericExpression
            });
        }
    }
}