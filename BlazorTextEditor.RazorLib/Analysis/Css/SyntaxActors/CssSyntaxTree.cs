using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxItems;
using BlazorTextEditor.RazorLib.Keyboard;
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
                    // Moved the assignment of token starting position index
                    // to inside the case itself therefore 1 character too far now so - 1.
                    pendingTokenStartingPositionIndex = stringWalker.PositionIndex - 1; 
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
                stringWalker.SkipWhitespace();
                
                // Comment start
                {
                    withinComment = stringWalker.CheckForSubstring(CssFacts.COMMENT_START);
                    if (withinComment)
                        return false;
                }

                var delimiters = WhitespaceFacts.ALL
                    .Union(new[]
                    {
                        KeyboardKeyFacts.PunctuationCharacters.OPEN_CURLY_BRACE,
                        KeyboardKeyFacts.PunctuationCharacters.CLOSE_CURLY_BRACE,
                    })
                    .ToList();

                // Set the expected text delimiters
                switch (expectedStyleBlockChild)
                {
                    case CssSyntaxKind.PropertyName:
                        delimiters.Add(CssFacts.PROPERTY_NAME_END);
                        break;
                    case CssSyntaxKind.PropertyValue:
                        delimiters.Add(CssFacts.PROPERTY_NAME_END);
                        break;
                }

                var startingPositionIndex = stringWalker.CurrentCharacter;

                while (stringWalker.CurrentCharacter != ParserFacts.END_OF_FILE &&
                       !delimiters.Contains(stringWalker.CurrentCharacter))
                {
                    _ = stringWalker.Consume();
                }

                if (stringWalker.PositionIndex - startingPositionIndex != 1)
                {
                    // did NOT immediately find EOF or a delimiter
                    // so create the corresponding token and add it to the list of results

                    var textSpan = new TextEditorTextSpan(
                        startingPositionIndex,
                        stringWalker.PositionIndex,
                        default);
                    
                    switch (expectedStyleBlockChild)
                    {
                        case CssSyntaxKind.PropertyName:
                        {
                            var propertyNameToken = new CssPropertyNameSyntax(
                                textSpan with
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
                                textSpan with
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