using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxTree
{
    public static JavaScriptSyntaxUnit ParseText(string content)
    {
        var documentChildren = new List<IJavaScriptSyntax>();
        var diagnosticBag = new TextEditorDiagnosticBag();
        
        var stringWalker = new StringWalker(content);

        while (!stringWalker.IsEof)
        {
            /*
             * string:
             *     if (currentCharacter == '"')
             *         var javaScriptStringSyntax = readString(stringWalker)
             *         documentChildren.Add(javaScriptStringSyntax);
             * comment:
             *     if (currentCharacter == '/')
             *         if (nextCharacter == '/')
             *             var javaScriptSingleLineCommentSyntax = readSingleLineComment();
             *             documentChildren.Add(javaScriptSingleLineCommentSyntax);
             *         else if (nextCharacter == '*')
             *            var javaScriptMultiLineCommentSyntax = readMultiLineComment();
             *            documentChildren.Add(javaScriptMultiLineCommentSyntax);
             * keyword:
             *     if (listOfKeywords.Contains(nextWord))
             *         var javaScriptKeywordSyntax = readKeyword();
             *         documentChildren.Add(javaScriptKeywordSyntax);
             */
            
            _ = stringWalker.Consume();
        }

        var javaScriptDocumentSyntax = new JavaScriptDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)JavaScriptDecorationKind.None),
            documentChildren.ToImmutableArray());
        
        return new JavaScriptSyntaxUnit(
            javaScriptDocumentSyntax,
            diagnosticBag);
    }
}