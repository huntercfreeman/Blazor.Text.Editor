using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxObjects;

public class JavaScriptDocumentSyntax : IJavaScriptSyntax
{
    public JavaScriptDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan, 
        ImmutableArray<IJavaScriptSyntax> children)
    {
        TextEditorTextSpan = textEditorTextSpan;
        Children = children;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJavaScriptSyntax> Children { get; }
    public JavaScriptSyntaxKind JavaScriptSyntaxKind => JavaScriptSyntaxKind.Document;
}