using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

public class CommentSyntax : TagSyntax
{
    public CommentSyntax(
        TextEditorTextSpan textEditorTextSpan) 
        : base(
            null,
            null,
            ImmutableArray<AttributeSyntax>.Empty, 
            ImmutableArray<IHtmlSyntax>.Empty, 
            TagKind.Text)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public override HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Comment;
    public override ImmutableArray<IHtmlSyntax> ChildHtmlSyntaxes => ImmutableArray<IHtmlSyntax>.Empty;
}