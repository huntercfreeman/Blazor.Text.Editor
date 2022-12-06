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
        _ = stringWalker.Consume();

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

        var startingPositionIndex = stringWalker.PositionIndex;
        
        // Enters the while loop on the '{'
        var seenCodeBlockStarts = 1;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_START)
                seenCodeBlockStarts++;
            
            if (stringWalker.CurrentCharacter == RazorFacts.CODE_BLOCK_END)
            {
                seenCodeBlockStarts--;

                if (seenCodeBlockStarts == 0)
                {
                    break;
                }
            }
        }
        
        // TODO: In the while loop extract all the C# code and use roslyn to syntax highlight it
        // TODO: In the while loop extract all the HTML code and use TextEditorHtmlLexer.cs to syntax highlight it
        return new List<TagSyntax>();
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Example: @myVariable
    /// </summary>
    private static List<TagSyntax> ReadImplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        throw new NotImplementedException();
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
                .ConsumeRange(matchedOn.Length);
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
        InjectedLanguageDefinition injectedLanguageDefinition, string matchedOn)
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
                .ConsumeRange(matchedOn.Length);
        }

        // TODO: See the "Perhaps a switch is needed here" comment. For now just return and revisit this.
        return injectedLanguageFragmentSyntaxes;

        // Perhaps a switch is needed here due to
        // the keywords having parameters which might vary in type?
        //
        // @page takes a string
        // what do the others take?
        // all others take a string or something else?
        //
        // switch (matchedOn)
        // {
        //     case RazorKeywords.PAGE_KEYWORD:
        //         break;
        //     case RazorKeywords.NAMESPACE_KEYWORD:
        //         break;
        //     case RazorKeywords.FUNCTIONS_KEYWORD:
        //         break;
        //     case RazorKeywords.INHERITS_KEYWORD:
        //         break;
        //     case RazorKeywords.MODEL_KEYWORD:
        //         break;
        //     case RazorKeywords.SECTION_KEYWORD:
        //         break;
        //     case RazorKeywords.HELPER_KEYWORD:
        //         break;
        // }
    }

    /// <summary>
    /// Example: @* This is a comment*@
    /// </summary>
    private static List<TagSyntax> ReadComment(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        throw new NotImplementedException();
    }
}