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
        var withinStyleBlock = false;
        var expectedStyleBlockChild = CssSyntaxKind.PropertyName;

        // pendingTokenStartingPositionIndex == -1
        // is to imply that there is no pending
        // token starting position index.
        var pendingTokenStartingPositionIndex = -1;

        stringWalker.WhileNotEndOfFile(() =>
        {
            if (withinComment)
            {
                // If the comment just started track its starting position index
                if (pendingTokenStartingPositionIndex == -1)
                {
                    pendingTokenStartingPositionIndex = stringWalker.PositionIndex;
                    
                    // Skip the rest of the comment opening text
                    _ = stringWalker.ConsumeRange(CssFacts.COMMENT_START.Length - 1);   
                }

                // Check comment end
                {
                    var closingOfCommentTextFound = stringWalker
                        .CheckForSubstring(CssFacts.COMMENT_END);

                    if (closingOfCommentTextFound)
                    {
                        // Skip the rest of the comment closing text
                        _ = stringWalker.ConsumeRange(CssFacts.COMMENT_END.Length - 1);

                        var commentTextSpan = new TextEditorTextSpan(
                            pendingTokenStartingPositionIndex,
                            stringWalker.PositionIndex + 1,
                            (byte)CssDecorationKind.Comment);                    
                    
                        var commentToken = new CssCommentSyntax(
                            commentTextSpan,
                            ImmutableArray<ICssSyntax>.Empty);

                        cssDocumentChildren.Add(commentToken);

                        pendingTokenStartingPositionIndex = -1;
                        withinComment = false;
                    }
                }
            }
            else if (withinStyleBlock)
            {
                stringWalker.MoveToNextWord();
                
                // Comment start
                {
                    withinComment = stringWalker.CheckForSubstring(CssFacts.COMMENT_START);
                    if (withinComment)
                        return false;
                }

                var wordTuple = stringWalker.ConsumeWord();

                // Check style block end
                {
                    var closingOfStyleBlockTextFound = stringWalker.CurrentCharacter == 
                                                       CssFacts.STYLE_BLOCK_END;
                    
                    if (closingOfStyleBlockTextFound)
                    {
                        withinStyleBlock = false;
                        return false;
                    }
                }

                // Get corresponding token
                switch (expectedStyleBlockChild)
                {
                    case CssSyntaxKind.PropertyName:
                    {
                        var propertyNameToken = new CssPropertyNameSyntax(
                            wordTuple.textSpan with
                            {
                                DecorationByte = (byte)CssDecorationKind.PropertyName
                            },
                            ImmutableArray<ICssSyntax>.Empty);
                        
                        cssDocumentChildren.Add(propertyNameToken);

                        expectedStyleBlockChild = CssSyntaxKind.PropertyValue;
                        
                        break;
                    }
                    case CssSyntaxKind.PropertyValue:
                    {
                        var propertyValueToken = new CssPropertyValueSyntax(
                            wordTuple.textSpan with
                            {
                                DecorationByte = (byte)CssDecorationKind.PropertyValue
                            },
                            ImmutableArray<ICssSyntax>.Empty);
                        
                        cssDocumentChildren.Add(propertyValueToken);

                        expectedStyleBlockChild = CssSyntaxKind.PropertyName;
                        
                        break;
                    }
                }
            }
            else
            {
                // Comment start
                {
                    withinComment = stringWalker.CheckForSubstring(CssFacts.COMMENT_START);
                    if (withinComment)
                        return false;
                }

                // Style Block start
                {
                    withinStyleBlock = stringWalker.CurrentCharacter == CssFacts.STYLE_BLOCK_START;
                    if (withinStyleBlock)
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