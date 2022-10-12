using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

namespace FictitiousLanguage.ClassLib.Classes.SyntaxNodes;

public abstract class SyntaxNode : ISyntaxNode
{
    public abstract SyntaxNodeKind Kind { get; }
    public abstract ImmutableArray<ISyntaxNode> GetChildSyntaxNodes { get; }
}