using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public interface ICssSyntax
{
    public CssSyntaxKind CssSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
}