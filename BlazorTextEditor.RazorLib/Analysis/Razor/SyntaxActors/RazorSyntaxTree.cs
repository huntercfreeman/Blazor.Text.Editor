using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.InjectedLanguage;
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
        _ = stringWalker.ReadCharacter();

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

        if (stringWalker.CheckForSubstringRange(
                CSharpRazorKeywords.ALL,
                out matchedOn) &&
            matchedOn is not null)
        {
            return ReadCSharpRazorKeyword(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition,
                matchedOn);
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
                injectedLanguageDefinition);
        }

        // TODO: Check for invalid expressions
        return ReadInlineExpression(
            stringWalker,
            textEditorHtmlDiagnosticBag,
            injectedLanguageDefinition);
    }

    private static List<TagSyntax> ReadCodeBlock(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Code Block
        //
        // NOTE: There might be a mixture
        // of C# and HTML in the Razor Code Blocks.
        //
        // NOTE:
        // -<text></text>
        //     Render multiple lines of text without rendering an HTML Element
        // -@:
        //     Render a single line of text without rendering an HTML Element
        // -ANY tag can be used within the razor code blocks
        //     Example: <div></div> or <MyBlazorComponent/>

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
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
                unmatchedCodeBlockStarts++;
            
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
                var codeBlockTagSyntaxes = ReadCodeBlock(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);

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
}