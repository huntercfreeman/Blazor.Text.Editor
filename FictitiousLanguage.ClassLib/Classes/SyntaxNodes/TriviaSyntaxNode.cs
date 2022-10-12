using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes.SyntaxTokens;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes;

internal sealed class TriviaSyntaxNode : SyntaxNode
{
    public TriviaSyntaxNode(WhitespaceSyntaxToken childWhitespaceSyntaxToken)
    {
        ChildWhitespaceSyntaxToken = childWhitespaceSyntaxToken;
    }

    public WhitespaceSyntaxToken ChildWhitespaceSyntaxToken { get; set; }
    public override SyntaxNodeKind Kind => SyntaxNodeKind.TriviaSyntaxNode;

    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes
    {
        get
        {
            return ImmutableArray.Create<ISyntaxNode>(new SyntaxNode[] { });
        }
    }
}