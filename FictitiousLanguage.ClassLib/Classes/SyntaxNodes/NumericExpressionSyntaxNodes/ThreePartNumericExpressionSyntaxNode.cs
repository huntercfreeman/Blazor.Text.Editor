using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxNodes.OperatorSyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal class ThreePartNumericExpressionSyntaxNode : NumericExpressionSyntaxNode
{
    public ThreePartNumericExpressionSyntaxNode(NumericExpressionSyntaxNode? leftExpression,
        OperatorSyntaxNode operatorSyntaxNode,
        NumericExpressionSyntaxNode? rightExpression)
    {
        LeftExpressionNode = leftExpression;
        OperatorSyntaxNode = operatorSyntaxNode;
        RightExpressionNode = rightExpression;
    }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.ThreePartNumericExpressionSyntaxNode;
    public NumericExpressionSyntaxNode? LeftExpressionNode { get; set; }
    public OperatorSyntaxNode OperatorSyntaxNode { get; set; }
    public NumericExpressionSyntaxNode? RightExpressionNode { get; set; }

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[]
            {
                LeftExpressionNode,
                OperatorSyntaxNode,
                RightExpressionNode,
            });
        }
    }
}