using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes;

public class CompilationUnit : SyntaxNode
{
    internal CompilationUnit()
    {
    }

    public List<ISyntaxNode> SyntaxNodes { get; set; } = new();

    public override SyntaxNodeKind Kind => SyntaxNodeKind.CompilationUnitSyntaxNode;
    public override ImmutableArray<ISyntaxNode> GetChildSyntaxNodes => SyntaxNodes.ToImmutableArray();
}