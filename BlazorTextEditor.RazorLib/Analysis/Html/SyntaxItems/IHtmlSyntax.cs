using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxItems;

public interface IHtmlSyntax
{
    public HtmlSyntaxKind HtmlSyntaxKind { get; }
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; }
}