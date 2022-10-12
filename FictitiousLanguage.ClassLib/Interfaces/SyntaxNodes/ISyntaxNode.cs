using System.Collections.Immutable;
using FictitiousLanguage.ClassLib.Classes;

namespace FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;

public interface ISyntaxNode
{
    public SyntaxNodeKind Kind { get; }
    public abstract ImmutableArray<ISyntaxNode> GetChildSyntaxNodes { get; }
}