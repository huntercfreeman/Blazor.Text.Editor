using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Facts;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxObjects;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;

public class FSharpSyntaxTree
{
    public static FSharpSyntaxUnit ParseText(string content)
    {
        var documentChildren = new List<IFSharpSyntax>();
        var diagnosticBag = new TextEditorDiagnosticBag();
        
        var stringWalker = new StringWalker(content);

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CurrentCharacter == FSharpFacts.STRING_STARTING_CHARACTER)
            {
                var fSharpStringSyntax = ReadString(stringWalker, diagnosticBag);
                
                documentChildren.Add(fSharpStringSyntax);
            }
            else if (stringWalker.CheckForSubstring(FSharpFacts.COMMENT_SINGLE_LINE_START))
            {
                var fSharpCommentSyntax = ReadCommentSingleLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(fSharpCommentSyntax);
            }
            else if (stringWalker.CheckForSubstring(FSharpFacts.COMMENT_MULTI_LINE_START))
            {
                var fSharpCommentSyntax = ReadCommentMultiLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(fSharpCommentSyntax);
            }
            else
            {
                if (TryReadKeyword(stringWalker, diagnosticBag, out var fSharpKeywordSyntax) &&
                    fSharpKeywordSyntax is not null)
                {
                    documentChildren.Add(fSharpKeywordSyntax);
                }
            }
            
            /*
             * keyword:
             *     if (listOfKeywords.Contains(nextWord))
             *         var fSharpKeywordSyntax = readKeyword();
             *         documentChildren.Add(fSharpKeywordSyntax);
             */
            
            _ = stringWalker.Consume();
        }

        var fSharpDocumentSyntax = new FSharpDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)FSharpDecorationKind.None),
            documentChildren.ToImmutableArray());
        
        return new FSharpSyntaxUnit(
            fSharpDocumentSyntax,
            diagnosticBag);
    }

    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="FSharpFacts.STRING_STARTING_CHARACTER"/>
    /// </summary>
    private static FSharpStringSyntax ReadString(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CurrentCharacter == FSharpFacts.STRING_ENDING_CHARACTER)
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)FSharpDecorationKind.Error));
        }

        var stringTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + 1,
            (byte)FSharpDecorationKind.String);
        
        return new FSharpStringSyntax(
            stringTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="FSharpFacts.COMMENT_SINGLE_LINE_START"/>
    /// </summary>
    private static FSharpStringSyntax ReadCommentSingleLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (FSharpFacts.COMMENT_SINGLE_LINE_ENDINGS.Contains(stringWalker.CurrentCharacter))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)FSharpDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)FSharpDecorationKind.Comment);
        
        return new FSharpStringSyntax(
            commentTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -<see cref="FSharpFacts.COMMENT_MULTI_LINE_START"/>
    /// </summary>
    private static FSharpStringSyntax ReadCommentMultiLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CheckForSubstring(FSharpFacts.COMMENT_MULTI_LINE_END))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)FSharpDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + FSharpFacts.COMMENT_MULTI_LINE_END.Length,
            (byte)FSharpDecorationKind.Comment);
        
        return new FSharpStringSyntax(
            commentTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -Any CurrentCharacter value is valid as this method is 'try'
    /// </summary>
    private static bool TryReadKeyword(StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag,
        out FSharpKeywordSyntax? fSharpKeywordSyntax)
    {
        var wordTuple = stringWalker.ConsumeWord();
            
        var foundKeyword = FSharpKeywords.ALL
            .FirstOrDefault(keyword =>
                keyword == wordTuple.value);
        
        if (foundKeyword is not null)
        {
            fSharpKeywordSyntax = new FSharpKeywordSyntax(
                wordTuple.textSpan with
                {
                    DecorationByte =
                    (byte)FSharpDecorationKind.Keyword
                });

            return true;
        }

        if (wordTuple.textSpan.StartingIndexInclusive != -1)
        {
            // backtrack by the length of the word as it was not an actual keyword.
            stringWalker.BacktrackRange(wordTuple.value.Length);
        }
        
        fSharpKeywordSyntax = null;
        return false;
    }
}