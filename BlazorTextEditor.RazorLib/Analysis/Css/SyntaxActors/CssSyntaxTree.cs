using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class CssSyntaxTree
{
    public static CssSyntaxUnit ParseText(string content)
    {
        // Items to return wrapped in a CssSyntaxUnit
        var cssDocumentChildren = new List<ICssSyntax>();
        var textEditorCssDiagnosticBag = new TextEditorCssDiagnosticBag();
        
        // Step through the string 'character by character'
        var stringWalker = new StringWalker(content);

        // States
        var withinComment = false;

        // pendingTokenStartingPositionIndex == -1
        // is to imply that there is no pending
        // token starting position index.
        var pendingTokenStartingPositionIndex = -1;
        
        stringWalker.WhileNotEndOfFile(() =>
        {
            if (withinComment)
            {
                var closingOfCommentTextFound = stringWalker
                    .CheckForSubstring(CssFacts.COMMENT_END);

                if (closingOfCommentTextFound)
                {
                    // Skip the rest of the comment closing text
                    _ = stringWalker.ConsumeRange(CssFacts.COMMENT_END.Length - 1);

                    var commentTextSpan = new TextEditorTextSpan(
                        pendingTokenStartingPositionIndex,
                        stringWalker.PositionIndex,
                        (byte)CssDecorationKind.Comment);                    
                    
                    var commentToken = new CssCommentSyntax(
                        commentTextSpan,
                        ImmutableArray<ICssSyntax>.Empty, 
                        stringWalker.GetText(commentTextSpan));

                    cssDocumentChildren.Add(commentToken);

                    pendingTokenStartingPositionIndex = -1;
                    withinComment = false;
                }
            }
            else
            {
                withinComment = stringWalker.CheckForSubstring(CssFacts.COMMENT_START);
                if (withinComment)
                {
                    // Opening text of a comment found

                    pendingTokenStartingPositionIndex = stringWalker.PositionIndex;
                    
                    // Skip the rest of the comment opening text
                    _ = stringWalker.ConsumeRange(CssFacts.COMMENT_START.Length - 1);

                    return false;
                }
            }
            
            return false;
        });

        var cssDocumentSyntax = new CssDocumentSyntax(
            new TextEditorTextSpan(
                0, 
                stringWalker.PositionIndex, 
                (byte)CssDecorationKind.None),
            cssDocumentChildren.ToImmutableArray());
        
        var cssSyntaxUnit = new CssSyntaxUnit(
            cssDocumentSyntax,
            textEditorCssDiagnosticBag);
        
        return cssSyntaxUnit;
    }
}