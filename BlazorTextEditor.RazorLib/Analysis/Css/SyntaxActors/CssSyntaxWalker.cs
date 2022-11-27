using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class CssSyntaxWalker
{
    public List<CssCommentSyntax> CssCommentSyntaxes { get; } = new();
    public List<CssPropertyNameSyntax> CssPropertyNameSyntaxes { get; } = new();
    public List<CssPropertyValueSyntax> CssPropertyValueSyntaxes { get; } = new();

    public void Visit(ICssSyntax cssSyntax)
    {
        foreach (var child in cssSyntax.ChildCssSyntaxes) 
            Visit(child);

        switch (cssSyntax.CssSyntaxKind)
        {
            case CssSyntaxKind.Comment:
                VisitCssCommentSyntax((CssCommentSyntax)cssSyntax);
                break;
            case CssSyntaxKind.PropertyName:
                VisitCssPropertyNameSyntax((CssPropertyNameSyntax)cssSyntax);
                break;
            case CssSyntaxKind.PropertyValue:
                VisitCssPropertyValueSyntax((CssPropertyValueSyntax)cssSyntax);
                break;
        }
    }

    public virtual void VisitCssCommentSyntax(CssCommentSyntax cssCommentSyntax)
    {
        CssCommentSyntaxes.Add(cssCommentSyntax);
    }
    
    public virtual void VisitCssPropertyNameSyntax(CssPropertyNameSyntax cssPropertyNameSyntax)
    {
        CssPropertyNameSyntaxes.Add(cssPropertyNameSyntax);
    }
    
    public virtual void VisitCssPropertyValueSyntax(CssPropertyValueSyntax cssPropertyValueSyntax)
    {
        CssPropertyValueSyntaxes.Add(cssPropertyValueSyntax);
    }
}