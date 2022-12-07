using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

public class CommentSyntax : IHtmlSyntax
{
    public CommentSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Comment;
    public ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IHtmlSyntax>.Empty;
}