using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;

public class CssCommentSyntax : ICssSyntax
{
    public CssCommentSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<ICssSyntax> childCssSyntaxes,
        string value)
    {
        ChildCssSyntaxes = childCssSyntaxes;
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
    
    /// <summary>
    /// TODO: Should <see cref="Value"/> be removed? Having this property would surely duplicate the source text and use memory right?
    /// </summary>
    public string Value { get; }
    
    public CssSyntaxKind CssSyntaxKind => CssSyntaxKind.Comment;
}