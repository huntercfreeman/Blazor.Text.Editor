using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

public class AttributeValueSyntax : IHtmlSyntax
{
    public AttributeValueSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.AttributeValue;
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IHtmlSyntax>.Empty;
}