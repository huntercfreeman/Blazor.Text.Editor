using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.Facts;
using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;
using BlazorTextEditor.RazorLib.Analysis.Razor.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;

public class RazorSyntaxTree
{
    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="RazorInjectedLanguageFacts.RazorInjectedLanguageDefinition.TransitionSubstring"/><br/>
    /// </summary>
    public static List<TagSyntax> ParseInjectedLanguageFragment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // <td>@forecast...</td>
        // <td>@forecast...</td>
        // <td>@forecast...</td>
        // <td>@forecast...</td>

        // current character is '@'
        _ = stringWalker.ReadCharacter();


        // current character is 'f'

        string? matchedOn = null;

        if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
        {
            textEditorHtmlDiagnosticBag.Report(
                DiagnosticLevel.Error,
                "Whitespace immediately following the Razor transition character is unexpected.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None));

            return new List<TagSyntax>();
        }

        
        
        if (stringWalker.CheckForSubstringRange(
                RazorKeywords.ALL,
                out matchedOn) &&
            matchedOn is not null)
        {
            return ReadRazorKeyword(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition,
                matchedOn);
        }
        
        // foreach (var forecast in forecasts)....
        
        // <td>@forecast.Property</td>
        // <td>@forecast.Property</td>
        // <td>@forecast.Property</td>
        // <td>@forecast.Property</td>
        
        // CurrentCharcter 'f'

        {
            var nextWord = stringWalker.PeekNextWord();

            string? foundString = null;

            foreach (var cSharpRazorKeyword in CSharpRazorKeywords.ALL)
            {
                if (cSharpRazorKeyword == nextWord)
                {
                    foundString = cSharpRazorKeyword;
                    break;
                }
            }

            if (foundString is not null)
            {
                return ReadCSharpRazorKeyword(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    foundString);
            }
        }

        if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_START)
        {
            return ReadComment(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }

        if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
        {
            return ReadCodeBlock(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition,
                false);
        }

        // TODO: Check for invalid expressions
        return ReadInlineExpression(
            stringWalker,
            textEditorHtmlDiagnosticBag,
            injectedLanguageDefinition);
    }

    /// <param name="isClassLevelCodeBlock">
    /// The @code{...} section must be wrapped in an adhoc class definition
    /// so that Roslyn can syntax highlight methods.
    /// <br/><br/>
    /// The @{...} code blocks must be wrapped in an adhoc method.
    /// <br/><br/>
    /// TODO: Is this true?
    /// </param>
    private static List<TagSyntax> ReadCodeBlock(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        bool isClassLevelCodeBlock)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();
        
        var startingPositionIndex = stringWalker.PositionIndex;
        
        // Syntax highlight the CODE_BLOCK_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentSyntax(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));
        }
        
        // Enters the while loop on the '{'
        var unmatchedCodeBlockStarts = 1;

        // While iterating through the text append any C# text to the cSharpBuilder
        // afterwards pass it through Roslyn for the syntax highlighting and map
        // the corresponding position indices.
        var cSharpBuilder = new StringBuilder();

        var positionIndexOffset = stringWalker.PositionIndex + 1;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();
            
            if (stringWalker.CurrentCharacter == HtmlFacts.OPEN_TAG_BEGINNING)
            {
                var positionIndexPriorToHtmlTag = stringWalker.PositionIndex;
                
                var tagSyntax = HtmlSyntaxTree.HtmlSyntaxTreeStateMachine.ParseTag(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);

                injectedLanguageFragmentSyntaxes.Add(tagSyntax);
                
                var necessaryWhitespacePadding = 
                    stringWalker.PositionIndex - 
                    positionIndexPriorToHtmlTag +
                    1;

                for (int i = 0; i < necessaryWhitespacePadding; i++)
                {
                    cSharpBuilder.Append(WhitespaceFacts.SPACE);
                }

                continue;
            }
            
            // '@:' is a single line version of '<text></text>' as of this comment | 2022-12-07
            var singleLineTextOutputText =
                $"{RazorFacts.TRANSITION_SUBSTRING}{RazorFacts.SINGLE_LINE_TEXT_OUTPUT_WITHOUT_ADDING_HTML_ELEMENT}";
            
            if (stringWalker.CheckForSubstring(singleLineTextOutputText))
            {
                var positionIndexPriorToHtmlTag = stringWalker.PositionIndex;
                
                _ = stringWalker.ReadRange(singleLineTextOutputText.Length - 1);
                
                while (!stringWalker.IsEof)
                {
                    _ = stringWalker.ReadCharacter();
                    
                    if (WhitespaceFacts.LINE_ENDING_CHARACTERS.Contains(stringWalker.CurrentCharacter))
                    {
                        break;
                    }
                }
                
                var necessaryWhitespacePadding = 
                    stringWalker.PositionIndex - 
                    positionIndexPriorToHtmlTag +
                    1;

                for (int i = 0; i < necessaryWhitespacePadding; i++)
                {
                    cSharpBuilder.Append(WhitespaceFacts.SPACE);
                }
                
                continue;
            }

            // Track all the C# text
            cSharpBuilder.Append(stringWalker.CurrentCharacter);
            
            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
            {
                unmatchedCodeBlockStarts++;
                continue;
            }
            
            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_END)
            {
                unmatchedCodeBlockStarts--;

                if (unmatchedCodeBlockStarts == 0)
                {
                    // Syntax highlight the CODE_BLOCK_END as a razor keyword specifically
                    {
                        injectedLanguageFragmentSyntaxes.Add(
                            new InjectedLanguageFragmentSyntax(
                                ImmutableArray<IHtmlSyntax>.Empty,
                                string.Empty,
                                new TextEditorTextSpan(
                                    stringWalker.PositionIndex,
                                    stringWalker.PositionIndex +
                                    1,
                                    (byte)HtmlDecorationKind.InjectedLanguageFragment)));
                    }

                    // A final '}' will be erroneously appended so remove that
                    cSharpBuilder.Remove(cSharpBuilder.Length - 1, 1);

                    var cSharpText = cSharpBuilder.ToString();

                    if (isClassLevelCodeBlock)
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(
                            ParseCSharpWithAdhocClassWrapping(
                                cSharpText,
                                positionIndexOffset));
                    }
                    else
                    {
                        injectedLanguageFragmentSyntaxes.AddRange(
                            ParseCSharpWithAdhocMethodWrapping(
                                cSharpText,
                                positionIndexOffset));
                    }
                    
                    break;
                }
            }
        }
        
        // TODO: In the while loop extract all the C# code and use roslyn to syntax highlight it
        // TODO: In the while loop extract all the HTML code and use TextEditorHtmlLexer.cs to syntax highlight it
        return injectedLanguageFragmentSyntaxes;
    }

    private static List<TagSyntax> ReadInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
        {
            return ReadExplicitInlineExpression(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }

        return ReadImplicitInlineExpression(
            stringWalker,
            textEditorHtmlDiagnosticBag,
            injectedLanguageDefinition);
    }

    /// <summary>
    /// Example: @(myVariable)
    /// </summary>
    private static List<TagSyntax> ReadExplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();
        
        var startingPositionIndex = stringWalker.PositionIndex;
        
        // Syntax highlight the EXPLICIT_EXPRESSION_START as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentSyntax(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        1,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));
        }
        
        // Enters the while loop on the '('
        var unmatchedExplicitExpressionStarts = 1;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
                unmatchedExplicitExpressionStarts++;
            
            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_END)
            {
                unmatchedExplicitExpressionStarts--;

                if (unmatchedExplicitExpressionStarts == 0)
                {
                    // Syntax highlight the EXPLICIT_EXPRESSION_END as a razor keyword specifically
                    {
                        injectedLanguageFragmentSyntaxes.Add(
                            new InjectedLanguageFragmentSyntax(
                                ImmutableArray<IHtmlSyntax>.Empty,
                                string.Empty,
                                new TextEditorTextSpan(
                                    stringWalker.PositionIndex,
                                    stringWalker.PositionIndex +
                                    1,
                                    (byte)HtmlDecorationKind.InjectedLanguageFragment)));
                    }
                    
                    break;
                }
            }
        }
        
        // TODO: Syntax highlighting
        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>
    /// Example: @myVariable
    /// </summary>
    private static List<TagSyntax> ReadImplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        return new List<TagSyntax>();
    }

    /// <summary>
    /// Example: @if (true) { ... } else { ... }
    /// </summary>
    private static List<TagSyntax> ReadCSharpRazorKeyword(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();
        
        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentSyntax(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        matchedOn.Length,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));

            _ = stringWalker
                .ReadRange(matchedOn.Length);
        }

        switch (matchedOn)
        {
            case CSharpRazorKeywords.CASE_KEYWORD:
                break;
            case CSharpRazorKeywords.DO_KEYWORD:
            {
                // Necessary in the case where the do-while statement's code block immediately follows the 'do' text
                // Example: "@do{"
                stringWalker.BacktrackCharacter();

                if (!TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.DO_KEYWORD,
                        out var codeBlockTagSyntaxes) ||
                    codeBlockTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                
                if (TryReadWhileOfDoWhile(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.DO_KEYWORD,
                        out var whileOfDoWhileTagSyntaxes) &&
                    whileOfDoWhileTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(whileOfDoWhileTagSyntaxes);
                }

                break;
            }
            case CSharpRazorKeywords.DEFAULT_KEYWORD:
                break;
            case CSharpRazorKeywords.FOR_KEYWORD:
            {
                // Necessary in the case where the switch statement's predicate expression immediately follows the 'switch' text
                // Example: "@switch(predicate) {"
                stringWalker.BacktrackCharacter();

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.FOR_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.FOR_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                }

                break;
            }
            case CSharpRazorKeywords.FOREACH_KEYWORD:
            {
                // Necessary in the case where the foreach statement's predicate expression immediately follows the 'foreach' text
                // Example: "@foreach(predicate) {"
                stringWalker.BacktrackCharacter();

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.FOREACH_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.FOREACH_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                }

                break;
            }
            case CSharpRazorKeywords.IF_KEYWORD:
            {
                // Necessary in the case where the if statement's predicate expression immediately follows the 'if' text
                // Example: "@if(predicate) {"
                stringWalker.BacktrackCharacter();

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                }

                var restorePositionIndexPriorToTryReadElseIf = stringWalker.PositionIndex;

                while (TryReadElseIf(
                           stringWalker,
                           textEditorHtmlDiagnosticBag,
                           injectedLanguageDefinition,
                           CSharpRazorKeywords.IF_KEYWORD,
                           out var elseIfTagSyntaxes) &&
                       elseIfTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(elseIfTagSyntaxes);
                    restorePositionIndexPriorToTryReadElseIf = stringWalker.PositionIndex;
                }

                if (restorePositionIndexPriorToTryReadElseIf != stringWalker.PositionIndex)
                {
                    stringWalker.BacktrackRange(
                        stringWalker.PositionIndex - restorePositionIndexPriorToTryReadElseIf);
                }

                if (TryReadElse(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var elseTagSyntaxes) &&
                    elseTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(elseTagSyntaxes);
                }

                break;
            }
            case CSharpRazorKeywords.ELSE_KEYWORD:
                break;
            case CSharpRazorKeywords.LOCK_KEYWORD:
                break;
            case CSharpRazorKeywords.SWITCH_KEYWORD:
            {
                // Necessary in the case where the switch statement's predicate expression immediately follows the 'switch' text
                // Example: "@switch(predicate) {"
                stringWalker.BacktrackCharacter();

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.SWITCH_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.SWITCH_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                }

                break;
            }
            case CSharpRazorKeywords.TRY_KEYWORD:
                break;
            case CSharpRazorKeywords.CATCH_KEYWORD:
                break;
            case CSharpRazorKeywords.FINALLY_KEYWORD:
                break;
            case CSharpRazorKeywords.USING_KEYWORD:
                break;
            case CSharpRazorKeywords.WHILE_KEYWORD:
            {
                // Necessary in the case where the while statement's predicate expression immediately follows the 'while' text
                // Example: "@while(predicate) {"
                stringWalker.BacktrackCharacter();

                if (!TryReadExplicitInlineExpression(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }

                injectedLanguageFragmentSyntaxes.AddRange(explicitExpressionTagSyntaxes);

                if (TryReadCodeBlock(
                        stringWalker,
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    injectedLanguageFragmentSyntaxes.AddRange(codeBlockTagSyntaxes);
                }

                break;
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>
    /// Example: @page "/counter"
    /// </summary>
    private static List<TagSyntax> ReadRazorKeyword(StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition, 
        string matchedOn)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();
        
        // Syntax highlight the keyword as a razor keyword specifically
        {
            injectedLanguageFragmentSyntaxes.Add(
                new InjectedLanguageFragmentSyntax(
                    ImmutableArray<IHtmlSyntax>.Empty,
                    string.Empty,
                    new TextEditorTextSpan(
                        stringWalker.PositionIndex,
                        stringWalker.PositionIndex +
                        matchedOn.Length,
                        (byte)HtmlDecorationKind.InjectedLanguageFragment)));

            _ = stringWalker
                .ReadRange(matchedOn.Length);
        }

        switch (matchedOn)
        {
            case RazorKeywords.PAGE_KEYWORD:
            {
                // @page "/csharp"
                break;
            }
            case RazorKeywords.NAMESPACE_KEYWORD:
            {
                // @namespace BlazorTextEditor.Demo.ServerSide.Pages
                break;
            }
            case RazorKeywords.CODE_KEYWORD:
            case RazorKeywords.FUNCTIONS_KEYWORD:
            {
                // Necessary in the case where the code block immediately follows any keyword's text
                // Example: "@code{" 
                stringWalker.BacktrackCharacter();

                var keywordText = matchedOn == RazorKeywords.CODE_KEYWORD
                    ? RazorKeywords.CODE_KEYWORD
                    : RazorKeywords.FUNCTIONS_KEYWORD;
                
                if (TryReadCodeBlock(
                        stringWalker, 
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        keywordText,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    return injectedLanguageFragmentSyntaxes
                        .Union(codeBlockTagSyntaxes)
                        .ToList();
                }
                
                break;
            }
            case RazorKeywords.INHERITS_KEYWORD:
            {
                // @inherits IconBase
                break;
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    /// <summary>
    /// Example: @* This is a comment*@
    /// </summary>
    private static List<TagSyntax> ReadComment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        // Enters the while loop on the '*'
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_END &&
               stringWalker.NextCharacter.ToString() == RazorFacts.TRANSITION_SUBSTRING)
            {
                break;
            }
        }
        
        // TODO: Syntax highlighting
        return new List<TagSyntax>();
    }
    
    private static bool TryReadCodeBlock(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<TagSyntax>? tagSyntaxes)
    {
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
            {
                var isClassLevelCodeBlock =
                    keywordText == RazorKeywords.CODE_KEYWORD ||
                    keywordText == RazorKeywords.FUNCTIONS_KEYWORD;
                
                var codeBlockTagSyntaxes = ReadCodeBlock(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition,
                    isClassLevelCodeBlock);

                tagSyntaxes = codeBlockTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;
                    
            textEditorHtmlDiagnosticBag.Report(
                DiagnosticLevel.Error,
                $"A code block was expected to follow the {RazorFacts.TRANSITION_SUBSTRING}{keywordText} razor keyword.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None));

            break;
        }

        tagSyntaxes = null;
        return false;
    }
    
    private static bool TryReadExplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<TagSyntax>? tagSyntaxes)
    {
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.EXPLICIT_EXPRESSION_START)
            {
                var explicitExpressionTagSyntaxes = ReadExplicitInlineExpression(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);

                tagSyntaxes = explicitExpressionTagSyntaxes;
                return true;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;
                    
            textEditorHtmlDiagnosticBag.Report(
                DiagnosticLevel.Error,
                $"An explicit expression predicate was expected to follow the {RazorFacts.TRANSITION_SUBSTRING}{keywordText} razor keyword.",
                new TextEditorTextSpan(
                    stringWalker.PositionIndex,
                    stringWalker.PositionIndex + 1,
                    (byte)HtmlDecorationKind.None));

            break;
        }

        tagSyntaxes = null;
        return false;
    }
    
    private static bool TryReadElseIf(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<TagSyntax>? tagSyntaxes)
    {
        tagSyntaxes = new List<TagSyntax>();
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            var elseIfKeywordCombo = 
                $"{CSharpRazorKeywords.ELSE_KEYWORD} {CSharpRazorKeywords.IF_KEYWORD}"; 
            
            if (stringWalker.CheckForSubstring(elseIfKeywordCombo))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                elseIfKeywordCombo.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    // -1 is in the case that "else{" instead of a space between "else" and "{"
                    _ = stringWalker
                        .ReadRange(elseIfKeywordCombo.Length - 1);
                }
                
                if (!TryReadExplicitInlineExpression(
                        stringWalker, 
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.IF_KEYWORD,
                        out var explicitExpressionTagSyntaxes) ||
                    explicitExpressionTagSyntaxes is null)
                {
                    break;
                }
                
                tagSyntaxes.AddRange(explicitExpressionTagSyntaxes);
                
                if (TryReadCodeBlock(
                        stringWalker, 
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(codeBlockTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = tagSyntaxes.Any()
            ? tagSyntaxes
            : null;
        
        return false;
    }
    
    private static bool TryReadElse(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<TagSyntax>? tagSyntaxes)
    {
        tagSyntaxes = new List<TagSyntax>();
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(CSharpRazorKeywords.ELSE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                CSharpRazorKeywords.ELSE_KEYWORD.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    // -1 is in the case that "else{" instead of a space between "else" and "{"
                    _ = stringWalker
                        .ReadRange(CSharpRazorKeywords.ELSE_KEYWORD.Length - 1);
                }
                
                if (TryReadCodeBlock(
                        stringWalker, 
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var codeBlockTagSyntaxes) &&
                    codeBlockTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(codeBlockTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }
    
    private static bool TryReadWhileOfDoWhile(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition,
        string keywordText,
        out List<TagSyntax>? tagSyntaxes)
    {
        tagSyntaxes = new List<TagSyntax>();
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CheckForSubstring(CSharpRazorKeywords.WHILE_KEYWORD))
            {
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    tagSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                stringWalker.PositionIndex,
                                stringWalker.PositionIndex +
                                CSharpRazorKeywords.WHILE_KEYWORD.Length,
                                (byte)HtmlDecorationKind.InjectedLanguageFragment)));

                    // -1 is in the case that "while()" instead of a space between "while" and "("
                    _ = stringWalker
                        .ReadRange(CSharpRazorKeywords.WHILE_KEYWORD.Length - 1);
                }
                
                if (TryReadExplicitInlineExpression(
                        stringWalker, 
                        textEditorHtmlDiagnosticBag,
                        injectedLanguageDefinition,
                        CSharpRazorKeywords.ELSE_KEYWORD,
                        out var explicitExpressionTagSyntaxes) &&
                    explicitExpressionTagSyntaxes is not null)
                {
                    tagSyntaxes.AddRange(explicitExpressionTagSyntaxes);
                    return true;
                }

                break;
            }

            if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                continue;

            break;
        }

        tagSyntaxes = null;
        return false;
    }

    private static List<TagSyntax> ParseCSharpWithAdhocClassWrapping(
        string cSharpText,
        int offsetPositionIndex)
    {
        var classTemplateOpening = "public class Aaa{";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            offsetPositionIndex);
    }
    
    private static List<TagSyntax> ParseCSharpWithAdhocMethodWrapping(
        string cSharpText,
        int offsetPositionIndex)
    {
        var classTemplateOpening = "public class Aaa{public void Bbb(){";

        var injectedLanguageString = classTemplateOpening +
                                     cSharpText;

        return ParseCSharp(
            injectedLanguageString,
            classTemplateOpening.Length,
            offsetPositionIndex);
    }
    
    /// <summary>
    /// If Lexing C# from a razor code block
    /// one must either use <see cref="ParseCSharpWithAdhocClassWrapping"/> for an @code{} section
    /// or <see cref="ParseCSharpWithAdhocMethodWrapping"/> for a basic @{} block
    /// </summary>
    private static List<TagSyntax> ParseCSharp(
        string cSharpText,
        int adhocTemplateOpeningLength,
        int offsetPositionIndex)
    {
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();
        
        var lexer = new TextEditorCSharpLexer();

        var lexedInjectedLanguage = lexer.Lex(
                cSharpText)
            .Result;

        foreach (var lexedTokenTextSpan in lexedInjectedLanguage)
        {
            var startingIndexInclusive = lexedTokenTextSpan.StartingIndexInclusive +
                                         offsetPositionIndex -
                                         adhocTemplateOpeningLength;

            var endingIndexExclusive = lexedTokenTextSpan.EndingIndexExclusive +
                                       offsetPositionIndex -
                                       adhocTemplateOpeningLength;

            // startingIndexInclusive < 0 means it was part of the class
            // template that was prepended so roslyn would recognize methods
            if (lexedTokenTextSpan.StartingIndexInclusive - adhocTemplateOpeningLength
                < 0)
                continue;

            var cSharpDecorationKind = (CSharpDecorationKind)lexedTokenTextSpan.DecorationByte;

            switch (cSharpDecorationKind)
            {
                case CSharpDecorationKind.None:
                    break;
                case CSharpDecorationKind.Method:
                    var razorMethodTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorMethodTextSpan));

                    break;
                case CSharpDecorationKind.Type:
                    var razorTypeTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorTypeTextSpan));

                    break;
                case CSharpDecorationKind.Parameter:
                    var razorVariableTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorVariableTextSpan));

                    break;
                case CSharpDecorationKind.StringLiteral:
                    var razorStringLiteralTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorStringLiteralTextSpan));

                    break;
                case CSharpDecorationKind.Keyword:
                    var razorKeywordTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageKeyword,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorKeywordTextSpan));

                    break;
                case CSharpDecorationKind.Comment:
                    var razorCommentTextSpan = lexedTokenTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.Comment,
                        StartingIndexInclusive = startingIndexInclusive,
                        EndingIndexExclusive = endingIndexExclusive,
                    };

                    injectedLanguageFragmentSyntaxes.Add(new InjectedLanguageFragmentSyntax(
                        ImmutableArray<IHtmlSyntax>.Empty,
                        string.Empty,
                        razorCommentTextSpan));

                    break;
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }
}