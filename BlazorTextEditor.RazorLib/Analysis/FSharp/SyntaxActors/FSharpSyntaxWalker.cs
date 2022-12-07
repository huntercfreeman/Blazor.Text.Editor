using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxEnums;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;

public class FSharpSyntaxWalker
{
    public List<FSharpStringSyntax> FSharpStringSyntaxes { get; } = new();
    public List<FSharpCommentSyntax> FSharpCommentSyntaxes { get; } = new();
    public List<FSharpKeywordSyntax> FSharpKeywordSyntaxes { get; } = new();

    public void Visit(IFSharpSyntax node)
    {
        foreach (var child in node.Children)
        {
            Visit(child);
        }

        switch (node.FSharpSyntaxKind)
        {
            case FSharpSyntaxKind.String:
                VisitFSharpStringSyntax((FSharpStringSyntax)node);
                break;
            case FSharpSyntaxKind.Comment:
                VisitFSharpCommentSyntax((FSharpCommentSyntax)node);
                break;
            case FSharpSyntaxKind.Keyword:
                VisitFSharpKeywordSyntax((FSharpKeywordSyntax)node);
                break;
        }
    }

    private void VisitFSharpStringSyntax(FSharpStringSyntax node)
    {
        FSharpStringSyntaxes.Add(node);
    }
    
    private void VisitFSharpCommentSyntax(FSharpCommentSyntax node)
    {
        FSharpCommentSyntaxes.Add(node);
    }
    
    private void VisitFSharpKeywordSyntax(FSharpKeywordSyntax node)
    {
        FSharpKeywordSyntaxes.Add(node);
    }
}