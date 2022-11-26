using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class CssSyntaxWalker
{
    public List<CssCommentSyntax> CssCommentSyntaxes { get; } = new();

    public void Visit(ICssSyntax cssSyntax)
    {
        foreach (var child in cssSyntax.ChildCssSyntaxes) 
            Visit(child);

        switch (cssSyntax.CssSyntaxKind)
        {
            case CssSyntaxKind.Comment:
                VisitCssCommentSyntax((CssCommentSyntax)cssSyntax);
                break;
        }
    }

    public virtual void VisitCssCommentSyntax(CssCommentSyntax cssCommentSyntax)
    {
        CssCommentSyntaxes.Add(cssCommentSyntax);
    }
}