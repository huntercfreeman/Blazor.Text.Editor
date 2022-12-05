using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptKeywordSyntax : IJavaScriptSyntax
{
    public JavaScriptKeywordSyntax(TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJavaScriptSyntax> Children => ImmutableArray<IJavaScriptSyntax>.Empty;
    public JavaScriptSyntaxKind JavaScriptSyntaxKind => JavaScriptSyntaxKind.Keyword;
}