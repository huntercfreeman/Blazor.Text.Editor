using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer;

public interface IGenericSyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IGenericSyntax> Children { get; }
    public GenericSyntaxKind GenericSyntaxKind { get; }
}