using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public interface IJavaScriptSyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJavaScriptSyntax> Children { get; }
    public JavaScriptSyntaxKind JavaScriptSyntaxKind { get; }
}