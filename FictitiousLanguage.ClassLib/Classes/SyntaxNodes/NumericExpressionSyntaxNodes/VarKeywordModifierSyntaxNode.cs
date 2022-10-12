using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens.Keywords;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal class VarKeywordModifierSyntaxNode : KeywordModifierSyntaxNode
{
    public VarKeywordModifierSyntaxNode(ISyntaxToken keywordSyntaxToken)
        : this((KeywordSyntaxToken)keywordSyntaxToken)
    {
    }
    
    public VarKeywordModifierSyntaxNode(KeywordSyntaxToken keywordSyntaxToken) 
        : base(keywordSyntaxToken)
    {
        KeywordSyntaxToken = keywordSyntaxToken;
    }

    public VariableBoundNumericExpressionSyntaxNode InnerVariableBoundNumericExpressionSyntaxNode { get; set; }
    public override SyntaxNodeKind Kind => SyntaxNodeKind.VarKeywordModifierSyntaxNode;

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[] { InnerVariableBoundNumericExpressionSyntaxNode });
        }
    }
}