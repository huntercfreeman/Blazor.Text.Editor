using System.Text;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp;

public class FSharpSyntaxTree
{
    public static List<TextEditorTextSpan> ParseText(string content)
    {
        var textEditorTextSpans = new List<TextEditorTextSpan>();
        
        var stringWalker = new StringWalker(content);
        
        var wordBuilder = new StringBuilder();

        var wordBuilderStartingIndexInclusive = -1;
        
        var possibleKeywordsState = FSharpKeywords.All.ToList();
        
        stringWalker.WhileNotEndOfFile(() =>
        {
            if (FSharpWhitespace.WHITESPACE
                .Contains(stringWalker.CurrentCharacter.ToString()))
            {
                // Check if wordBuilder contains a keyword

                // Using .First as I am proceeding the presumption
                // that .First() will short circuit and not evaluate
                // the predicate on the remaining keywords.
                //
                // Whereas I presume .Single would have to Assert there
                // was only one match by iterating through all the keywords.
                //
                // Not sure however if the keyword string comparisons
                // would even be impactful enough on performance
                // for this to be useful if it is true.
                var foundKeyword = possibleKeywordsState
                    .FirstOrDefault(keyword => 
                        keyword == wordBuilder.ToString());
                
                if (foundKeyword is not null)
                {
                    textEditorTextSpans.Add(
                        new TextEditorTextSpan(
                            wordBuilderStartingIndexInclusive,
                            stringWalker.PositionIndex,
                            (byte)FSharpDecorationKind.Keyword));
                }
                
                ResetFoundKeywordState();
            }
            else
            {
                if (wordBuilderStartingIndexInclusive == -1)
                {
                    // This is the start of a word
                    // as opposed to the continuation of a word

                    wordBuilderStartingIndexInclusive = stringWalker.PositionIndex;
                }
                
                wordBuilder.Append(stringWalker.CurrentCharacter);
                
                possibleKeywordsState = possibleKeywordsState
                    .Where(keyword =>
                        keyword.StartsWith(wordBuilder.ToString()))
                    .ToList();
            }

            return false;
        });
        
        // When the end of the file is found
        // the final keyword, if there is no whitespace after it,
        // the keyword will not be added to the list without this code
        {
            var foundKeyword = possibleKeywordsState
                .FirstOrDefault(keyword =>
                    keyword == wordBuilder.ToString());
            
            if (foundKeyword is not null)
            {
                textEditorTextSpans.Add(
                    new TextEditorTextSpan(
                        wordBuilderStartingIndexInclusive,
                        stringWalker.PositionIndex,
                        (byte)FSharpDecorationKind.Keyword));
                
                ResetFoundKeywordState();
            }
        }

        return textEditorTextSpans;

        void ResetFoundKeywordState()
        {
            wordBuilder.Clear();

            wordBuilderStartingIndexInclusive = -1;
            
            possibleKeywordsState = FSharpKeywords.All.ToList();
        }
    }
}