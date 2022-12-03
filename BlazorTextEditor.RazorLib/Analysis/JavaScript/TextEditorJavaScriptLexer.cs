using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class TextEditorJavaScriptLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var javaScriptSyntaxUnit = 
            JavaScriptSyntaxTree.ParseText(text);
        
        /*
         * JavaScriptSyntaxWalker.Visit(javaScriptSyntaxUnit.JavaScriptDocumentSyntax)
         *
         * public List<CommentSyntax> CommentSyntaxes { get; set; } = new(); 
         * 
         * Visit(IJavaScriptSyntax syntax)
         * {
         *     foreach (var child in syntax.Children)
         *          Visit(child);
         *     
         *     switch (syntax.JavaScriptSyntaxKind)
         *     {
         *          case Comment:
         *              VisitComment((JavaScriptCommentSyntax)syntax);
         *          case String:
         *              VisitString();
         *          case Keyword:
         *              VisitKeyword();
         *     }
         * }
         *
         * virtual VisitComment(JavaScriptCommentSyntax node)
         * {
         *     CommentSyntaxes.Add(node);
         * }
         *
         * virtual VisitString(JavaScriptSyntax node)
         * {
         *     AaaSyntaxes.Add(node);
         * }
         *
         * virtual VisitKeyword(JavaScriptSyntax node)
         * {
         *     AaaSyntaxes.Add(node);
         * }
         *
         *
         * return JavaScriptSyntaxWalker.CommentSyntaxes
         *     .Select(x => x.TextEditorTextSpan)
         *     .ToImmutableArray()
         */

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}