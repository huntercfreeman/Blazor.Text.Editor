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
            if (stringWalker.CurrentCharacter == JavaScriptFacts.STRING_STARTING_CHARACTER)
            {
                var javaScriptStringSyntax = ReadString(stringWalker, diagnosticBag);
                
                documentChildren.Add(javaScriptStringSyntax);
            }
            
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

    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="JavaScriptFacts.STRING_STARTING_CHARACTER"/>
    /// </summary>
    private static JavaScriptStringSyntax ReadString(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        // var example = "apple";

        var startingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CurrentCharacter == JavaScriptFacts.STRING_ENDING_CHARACTER)
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JavaScriptDecorationKind.Error));
        }

        var stringTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)JavaScriptDecorationKind.String);
        
        return new JavaScriptStringSyntax(
            stringTextEditorTextSpan);
    }
}