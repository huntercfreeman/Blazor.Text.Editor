using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

public class HtmlSyntaxWalker : XmlSyntaxWalker
{
    public List<AttributeNameSyntax> AttributeNameSyntaxes { get; } = new();
    public List<AttributeValueSyntax> AttributeValueSyntaxes { get; } = new();
    public List<InjectedLanguageFragmentSyntax> InjectedLanguageFragmentSyntaxes { get; } = new();
    public List<TagNameSyntax> TagNameSyntaxes { get; } = new();
    public List<CommentSyntax> CommentSyntaxes { get; } = new();
    public List<TagSyntax> TagSyntaxes { get; } = new();
    
    public override void VisitAttributeNameSyntax(AttributeNameSyntax attributeNameSyntax)
    {
        AttributeNameSyntaxes.Add(attributeNameSyntax);
        base.VisitAttributeNameSyntax(attributeNameSyntax);
    }

    public override void VisitAttributeValueSyntax(AttributeValueSyntax attributeValueSyntax)
    {
        AttributeValueSyntaxes.Add(attributeValueSyntax);
        base.VisitAttributeValueSyntax(attributeValueSyntax);
    }

    public override void VisitInjectedLanguageFragmentSyntax(InjectedLanguageFragmentSyntax injectedLanguageFragmentSyntax)
    {
        InjectedLanguageFragmentSyntaxes.Add(injectedLanguageFragmentSyntax);
        base.VisitInjectedLanguageFragmentSyntax(injectedLanguageFragmentSyntax);
    }

    public override void VisitTagNameSyntax(TagNameSyntax tagNameSyntax)
    {
        TagNameSyntaxes.Add(tagNameSyntax);
        base.VisitTagNameSyntax(tagNameSyntax);
    }

    public override void VisitCommentSyntax(CommentSyntax commentSyntax)
    {
        CommentSyntaxes.Add(commentSyntax);
        base.VisitCommentSyntax(commentSyntax);
    }
    
    public override void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
        base.VisitTagSyntax(tagSyntax);
    }
}