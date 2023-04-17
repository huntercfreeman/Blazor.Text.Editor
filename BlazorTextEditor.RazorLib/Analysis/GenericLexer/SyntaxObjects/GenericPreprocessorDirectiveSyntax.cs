using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxObjects;

public class GenericPreprocessorDirectiveSyntax : IGenericSyntax
{
    public GenericPreprocessorDirectiveSyntax(
        TextEditorTextSpan textEditorTextSpan, 
        ImmutableArray<IGenericSyntax> children)
    {
        TextEditorTextSpan = textEditorTextSpan;
        Children = children;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IGenericSyntax> Children { get; }
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.PreprocessorDirective;
}