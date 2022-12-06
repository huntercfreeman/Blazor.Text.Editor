using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;

namespace BlazorTextEditor.RazorLib.Analysis.Html;

public interface IHtmlSyntax
{
    public HtmlSyntaxKind HtmlSyntaxKind { get; }
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; }
}