using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

public class AttributeSyntax : IHtmlSyntax
{
    public AttributeSyntax(
        AttributeNameSyntax attributeNameSyntax,
        AttributeValueSyntax attributeValueSyntax)
    {
        AttributeNameSyntax = attributeNameSyntax;
        AttributeValueSyntax = attributeValueSyntax;
    }

    public AttributeNameSyntax AttributeNameSyntax { get; }
    public AttributeValueSyntax AttributeValueSyntax { get; }
    
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Attribute;
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IHtmlSyntax>.Empty;
}