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
        var startingPositionIndex = stringWalker.PositionIndex;
        
        // Enters the while loop on the '('
        var seenExplicitExpressionStarts = 1;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
                seenExplicitExpressionStarts++;
            
            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_END)
            {
                seenExplicitExpressionStarts--;

                if (seenExplicitExpressionStarts == 0)
                    break;
            }
        }
        
        // TODO: Syntax highlighting
        return new List<TagSyntax>();
    }

    /// <summary>
    /// Example: @myVariable
    /// </summary>
    private static List<TagSyntax> ReadImplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        // TODO: This method needs to break the a while loop on whitespace EXCEPT in the cases where the ending is obvious such as a method invocation
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

        // TODO: See the "I expect to use a switch here" comment. For now just return and revisit this.
        return injectedLanguageFragmentSyntaxes;

        // I expect to use a switch here
        //
        // an if statement for example would require
        // a check for an (else if)/(else) block
        //
        // switch (matchedOn)
        // {
        //     case CSharpRazorKeywords.CASE_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.DO_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.DEFAULT_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.FOR_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.FOREACH_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.IF_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.ELSE_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.LOCK_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.SWITCH_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.TRY_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.CATCH_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.FINALLY_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.USING_KEYWORD:
        //         break;
        //     case CSharpRazorKeywords.WHILE_KEYWORD:
        //         break;
        // }
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
                // In the case of "@code{" where the brace deliminating
                // the code block immediately follows the keyword text
                //
                // backtracking to the 'e' of 'code' is necessary
                // as the while loop immediately will read the next character
                // and otherwise miss the '{'
                stringWalker.BacktrackCharacter();
                
                while (!stringWalker.IsEof)
                {
                    _ = stringWalker.ReadCharacter();

                    if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
                    {
                        var codeBlockTagSyntaxes = ReadCodeBlock(
                            stringWalker,
                            textEditorHtmlDiagnosticBag,
                            injectedLanguageDefinition);

                        return injectedLanguageFragmentSyntaxes
                            .Union(codeBlockTagSyntaxes)
                            .ToList();
                    }

                    if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                        continue;
                    
                    var keywordText = matchedOn == RazorKeywords.CODE_KEYWORD
                        ? RazorKeywords.CODE_KEYWORD
                        : RazorKeywords.FUNCTIONS_KEYWORD;

                    textEditorHtmlDiagnosticBag.Report(
                        DiagnosticLevel.Error,
                        $"A code block was expected to follow the {RazorFacts.TRANSITION_SUBSTRING}{keywordText} razor keyword.",
                        new TextEditorTextSpan(
                            stringWalker.PositionIndex,
                            stringWalker.PositionIndex + 1,
                            (byte)HtmlDecorationKind.None));

                    break;
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
}