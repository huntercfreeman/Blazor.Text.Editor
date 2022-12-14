using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxObjects;

public class JavaScriptCommentSyntax : IJavaScriptSyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJavaScriptSyntax> Children => ImmutableArray<IJavaScriptSyntax>.Empty;
    public JavaScriptSyntaxKind JavaScriptSyntaxKind => JavaScriptSyntaxKind.Comment;
}