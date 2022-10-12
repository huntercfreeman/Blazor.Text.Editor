using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens.Keywords;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes.NumericExpressionSyntaxNodes;

internal class KeywordModifierSyntaxNode : SyntaxNode
{
    public KeywordModifierSyntaxNode(ISyntaxToken keywordSyntaxToken)
        : this((KeywordSyntaxToken)keywordSyntaxToken)
    {
    }
    
    public KeywordModifierSyntaxNode(KeywordSyntaxToken keywordSyntaxToken)
    {
        KeywordSyntaxToken = keywordSyntaxToken;
    }

    public override SyntaxNodeKind Kind => SyntaxNodeKind.KeywordModifierSyntaxNode;
    public KeywordSyntaxToken KeywordSyntaxToken { get; set; }

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[] {});
        }
    }
}