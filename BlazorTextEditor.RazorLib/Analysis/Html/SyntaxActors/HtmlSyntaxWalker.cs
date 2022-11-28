using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

public class HtmlSyntaxWalker
{
    public List<AttributeNameSyntax> AttributeNameSyntaxes { get; } = new();
    public List<AttributeValueSyntax> AttributeValueSyntaxes { get; } = new();
    public List<InjectedLanguageFragmentSyntax> InjectedLanguageFragmentSyntaxes { get; } = new();
    public List<TagNameSyntax> TagNameSyntaxes { get; } = new();
    public List<TagSyntax> TagSyntaxes { get; } = new();
    
    public void Visit(IHtmlSyntax syntaxNode)
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
        }
    }

    public void VisitAttributeNameSyntax(AttributeNameSyntax attributeNameSyntax)
    {
        AttributeNameSyntaxes.Add(attributeNameSyntax);
    }

    public void VisitAttributeValueSyntax(AttributeValueSyntax attributeValueSyntax)
    {
        AttributeValueSyntaxes.Add(attributeValueSyntax);
    }

    public void VisitInjectedLanguageFragmentSyntax(InjectedLanguageFragmentSyntax injectedLanguageFragmentSyntax)
    {
        InjectedLanguageFragmentSyntaxes.Add(injectedLanguageFragmentSyntax);
    }

    public void VisitTagNameSyntax(TagNameSyntax tagNameSyntax)
    {
        TagNameSyntaxes.Add(tagNameSyntax);
    }

    public void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
    }
}