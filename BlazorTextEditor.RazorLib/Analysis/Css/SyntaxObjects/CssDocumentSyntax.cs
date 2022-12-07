using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxObjects;

public class CssDocumentSyntax : ICssSyntax
{
    public CssDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<ICssSyntax> childCssSyntaxes)
    {
        ChildCssSyntaxes = childCssSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
    
    public CssSyntaxKind CssSyntaxKind => CssSyntaxKind.Document;
}