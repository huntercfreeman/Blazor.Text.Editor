using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;

public class CssIdentifierSyntax : ICssSyntax
{
    public CssIdentifierSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<ICssSyntax> childCssSyntaxes)
    {
        ChildCssSyntaxes = childCssSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
    
    public CssSyntaxKind CssSyntaxKind => CssSyntaxKind.Identifier;
}