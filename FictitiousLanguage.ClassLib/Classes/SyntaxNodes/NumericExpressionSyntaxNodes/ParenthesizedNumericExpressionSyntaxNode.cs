using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal class ParenthesizedNumericExpressionSyntaxNode : NumericExpressionSyntaxNode
{
    public ParenthesizedNumericExpressionSyntaxNode(ISyntaxToken syntaxToken)
        : this((OpenParenthesisSyntaxToken)syntaxToken)
    {
    }
    
    public ParenthesizedNumericExpressionSyntaxNode(OpenParenthesisSyntaxToken openParenthesis)
    {
        OpenParenthesis = openParenthesis;
    }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.ParenthesizedNumericExpressionSyntaxNode;
    public OpenParenthesisSyntaxToken OpenParenthesis { get; set; }
    public NumericExpressionSyntaxNode? InnerNumericExpression { get; set; }
    public CloseParenthesisSyntaxToken? CloseParenthesis { get; set; }

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