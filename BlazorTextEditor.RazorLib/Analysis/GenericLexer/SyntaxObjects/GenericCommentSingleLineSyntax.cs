using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxObjects;

public class GenericCommentSingleLineSyntax : IGenericSyntax
{
    public GenericCommentSingleLineSyntax(TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IGenericSyntax> Children => ImmutableArray<IGenericSyntax>.Empty;
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.CommentSingleLine;
}