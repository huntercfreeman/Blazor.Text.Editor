using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxObjects;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.GenericLexer.SyntaxActors;

public class GenericSyntaxTree
{
    public GenericSyntaxTree(GenericLanguageDefinition genericLanguageDefinition)
    {
        GenericLanguageDefinition = genericLanguageDefinition;
    }
    
    public GenericLanguageDefinition GenericLanguageDefinition { get; }
    
    public virtual GenericSyntaxUnit ParseText(string content)
    {
        var documentChildren = new List<IGenericSyntax>();
        var diagnosticBag = new TextEditorDiagnosticBag();
        
        var stringWalker = new StringWalker(content);

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CheckForSubstring(GenericLanguageDefinition.StringStart))
            {
                var genericStringSyntax = ParseString(stringWalker, diagnosticBag);
                
                documentChildren.Add(genericStringSyntax);
            }
            else if (stringWalker.CheckForSubstring(GenericLanguageDefinition.CommentSingleLineStart))
            {
                var genericCommentSingleLineSyntax = ParseCommentSingleLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(genericCommentSingleLineSyntax);
            }
            else if (stringWalker.CheckForSubstring(GenericLanguageDefinition.CommentMultiLineStart))
            {
                var genericCommentMultiLineSyntax = ParseCommentMultiLine(stringWalker, diagnosticBag);
                
                documentChildren.Add(genericCommentMultiLineSyntax);
            }
            else if (stringWalker.CheckForSubstring(GenericLanguageDefinition.FunctionInvocationStart))
            {
                if (TryParseFunctionIdentifier(stringWalker, diagnosticBag, out var genericFunctionSyntax) &&
                    genericFunctionSyntax is not null)
                {
                    documentChildren.Add(genericFunctionSyntax);
                }
            }
            else
            {
                if (TryParseKeyword(stringWalker, diagnosticBag, out var genericKeywordSyntax) &&
                    genericKeywordSyntax is not null)
                {
                    documentChildren.Add(genericKeywordSyntax);
                }
            }
            
            _ = stringWalker.ReadCharacter();
        }

        var genericDocumentSyntax = new GenericDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)GenericDecorationKind.None),
            documentChildren.ToImmutableArray());
        
        return new GenericSyntaxUnit(
            genericDocumentSyntax,
            diagnosticBag);
    }
    
    public virtual GenericCommentSingleLineSyntax ParseCommentSingleLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstringRange(
                    GenericLanguageDefinition.CommentSingleLineEndings,
                    out _))
            {
                break;
            }
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)GenericDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.CommentSingleLine);
        
        return new GenericCommentSingleLineSyntax(
            commentTextEditorTextSpan);
    }
    
    public virtual GenericCommentMultiLineSyntax ParseCommentMultiLine(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(GenericLanguageDefinition.CommentMultiLineEnd))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)GenericDecorationKind.Error));
        }

        var commentTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + GenericLanguageDefinition.CommentMultiLineEnd.Length,
            (byte)GenericDecorationKind.CommentMultiLine);
        
        return new GenericCommentMultiLineSyntax(
            commentTextEditorTextSpan);
    }
    
    public virtual GenericStringSyntax ParseString(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(GenericLanguageDefinition.StringEnd))
                break;
        }

        if (stringWalker.IsEof)
        {
            diagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)GenericDecorationKind.Error));
        }

        var stringTextEditorTextSpan = new TextEditorTextSpan(
            startingPositionIndex,
            stringWalker.PositionIndex + 1,
            (byte)GenericDecorationKind.String);
        
        return new GenericStringSyntax(
            stringTextEditorTextSpan);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// -Any CurrentCharacter value is valid as this method is 'try'
    /// </summary>
    private bool TryParseKeyword(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag,
        out GenericKeywordSyntax? genericKeywordSyntax)
    {
        var wordTuple = stringWalker.ConsumeWord();
            
        var foundKeyword = GenericLanguageDefinition.Keywords
            .FirstOrDefault(keyword =>
                keyword == wordTuple.value);
        
        if (foundKeyword is not null)
        {
            genericKeywordSyntax = new GenericKeywordSyntax(
                wordTuple.textSpan with
                {
                    DecorationByte =
                    (byte)GenericDecorationKind.Keyword
                });

            return true;
        }

        if (wordTuple.textSpan.StartingIndexInclusive != -1)
        {
            // backtrack by the length of the word as it was not an actual keyword.
            stringWalker.BacktrackRange(wordTuple.value.Length);
        }
        
        genericKeywordSyntax = null;
        return false;
    }
    
    private bool TryParseFunctionIdentifier(
        StringWalker stringWalker,
        TextEditorDiagnosticBag diagnosticBag,
        out GenericFunctionSyntax? genericFunctionSyntax)
    {
        /*
         * var obj = new Object();
         *
         * obj.Function();
         *
         * -----------------------
         *
         * Algorithm:
         *     Found '(', so read the previous word (skip any whitespace).
         *     If previous word is a valid function identifier,
         *     then assume it is so.
         */
        
        /*
         * var z = 2 + 4(3 - 1)
         *
         * Distribution of a number over parenthesis should not
         * get syntax highlighted as a function identifier.
         */

        var rememberPositionIndex = stringWalker.PositionIndex;
        
        bool startedReadingWord = false;

        var wordBuilder = new StringBuilder();
        
        // Enter here at '('
        while (!stringWalker.IsEof)
        {
            var backtrackedToCharacter = stringWalker.BacktrackCharacter();

            if (backtrackedToCharacter == ParserFacts.END_OF_FILE)
                break;
            
            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
            {
                if (startedReadingWord)
                    break;
            }
            else if (stringWalker.CheckForSubstring(GenericLanguageDefinition.MemberAccessToken) ||
                     stringWalker.CheckForSubstring(GenericLanguageDefinition.FunctionInvocationEnd))
            {
                break;
            }
            else
            {
                startedReadingWord = true;
                wordBuilder.Insert(0, stringWalker.CurrentCharacter);
            }
        }

        var word = wordBuilder.ToString();
        
        if (word.Length == 0 || char.IsDigit(word[0]))
        {
            genericFunctionSyntax = null;

            _ = stringWalker.ReadRange(
                rememberPositionIndex - stringWalker.PositionIndex);
            
            return false;
        }
        
        genericFunctionSyntax = new GenericFunctionSyntax(
            new TextEditorTextSpan(
                stringWalker.PositionIndex + 1,
                rememberPositionIndex,
                (byte)GenericDecorationKind.Function));
        
        _ = stringWalker.ReadRange(
            rememberPositionIndex - stringWalker.PositionIndex);
        
        return true;
    }
}