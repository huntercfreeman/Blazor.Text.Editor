using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

public abstract class XmlSyntaxWalker
{
    public virtual void Visit(IHtmlSyntax syntaxNode)
    {
        foreach (var child in syntaxNode.ChildHtmlSyntaxes) Visit(child);

        switch (syntaxNode.HtmlSyntaxKind)
        {
            case HtmlSyntaxKind.Tag:
            case HtmlSyntaxKind.InjectedLanguageFragment:
            case HtmlSyntaxKind.TagText:
            {
                var tagSyntax = (TagSyntax)syntaxNode;
                
                if (tagSyntax.OpenTagNameSyntax is not null)
                    VisitTagNameSyntax(tagSyntax.OpenTagNameSyntax);

                if (tagSyntax.CloseTagNameSyntax is not null)
                    VisitTagNameSyntax(tagSyntax.CloseTagNameSyntax);

                if (tagSyntax.TagKind == TagKind.InjectedLanguageCodeBlock)
                {
                    VisitInjectedLanguageFragmentSyntax(
                        (InjectedLanguageFragmentSyntax)tagSyntax);
                }
                else if (tagSyntax.TagKind == TagKind.Opening || 
                         tagSyntax.TagKind == TagKind.SelfClosing)
                {
                    VisitTagSyntax(tagSyntax);
                }
                
                foreach (var attributeSyntax in tagSyntax.AttributeSyntaxes)
                    Visit(attributeSyntax);
                
                break;
            }
            case HtmlSyntaxKind.Attribute:
            {
                var attributeSyntax = (AttributeSyntax)syntaxNode;

                VisitAttributeNameSyntax(attributeSyntax.AttributeNameSyntax);
                VisitAttributeValueSyntax(attributeSyntax.AttributeValueSyntax);
                
                break;
            }
            case HtmlSyntaxKind.Comment:
            {
                var commentSyntax = (CommentSyntax)syntaxNode;

                VisitCommentSyntax(commentSyntax);
                
                break;
            }
        }
    }
    
    public virtual void VisitAttributeNameSyntax(
        AttributeNameSyntax attributeNameSyntax)
    {
        
    }

    public virtual void VisitAttributeValueSyntax(
        AttributeValueSyntax attributeValueSyntax)
    {
        
    }

    public virtual void VisitInjectedLanguageFragmentSyntax(
        InjectedLanguageFragmentSyntax injectedLanguageFragmentSyntax)
    {
        
    }

    public virtual void VisitTagNameSyntax(
        TagNameSyntax tagNameSyntax)
    {
        
    }

    public virtual void VisitCommentSyntax(
        CommentSyntax commentSyntax)
    {
        
    }
    
    public virtual void VisitTagSyntax(
        TagSyntax tagSyntax)
    {
        
    }
}