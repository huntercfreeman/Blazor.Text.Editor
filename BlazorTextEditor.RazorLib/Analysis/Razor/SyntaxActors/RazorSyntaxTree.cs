using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
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
        var injectedLanguageFragmentSyntaxes = new List<TagSyntax>();

        var codeBlockOrExpressionStartingPositionIndex = stringWalker.PositionIndex;

        var foundCodeBlock = false;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            string? matchedOn = null;

            if (stringWalker.CheckForSubstringRange(
                    RazorKeywords.ALL,
                    out matchedOn) &&
                matchedOn is not null)
            {
                var aaa = ReadRazorKeyword(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
            else if (stringWalker.CheckForSubstringRange(
                         CSharpRazorKeywords.ALL,
                         out matchedOn) &&
                     matchedOn is not null)
            {
                var aaa = ReadCSharpRazorKeyword(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
            else if (stringWalker.CurrentCharacter == RazorFacts.COMMENT_START)
            {
                var aaa = ReadComment(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
            else if (true)
            {
                var aaa = ReadInlineExpression(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
            else
            {
                var aaa = ReadCodeBlock(
                    stringWalker,
                    textEditorHtmlDiagnosticBag,
                    injectedLanguageDefinition);
            }
        }

        return injectedLanguageFragmentSyntaxes;
    }

    private static object ReadCodeBlock(
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
    }

    private static object ReadInlineExpression(
        StringWalker stringWalker, 
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag, 
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Inline Expression
        //
        // Example: @myVariable
        // Example: @(myVariable)

        if (true)
        {
            var aaa = ReadImplicitInlineExpression(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }
        else
        {
            var aaa = ReadExplicitInlineExpression(
                stringWalker,
                textEditorHtmlDiagnosticBag,
                injectedLanguageDefinition);
        }
    }

    private static object ReadImplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Implicit Inline Expression
        //
        // Example: @myVariable
    }
    
    private static object ReadExplicitInlineExpression(
        StringWalker stringWalker,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Explicit Inline Expression
        //
        // Example: @(myVariable)
    }

    private static object ReadCSharpRazorKeyword(
        StringWalker stringWalker, 
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag, 
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // C# Razor Keyword
                //
                // Example: if (true) { ... } else { ... }
                
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                codeBlockOrExpressionStartingPositionIndex,
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

    private static object ReadRazorKeyword(
        StringWalker stringWalker, 
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag, 
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Keyword
                //
                // @page "/counter"
                
                // Syntax highlight the keyword as a razor keyword specifically
                {
                    injectedLanguageFragmentSyntaxes.Add(
                        new InjectedLanguageFragmentSyntax(
                            ImmutableArray<IHtmlSyntax>.Empty,
                            string.Empty,
                            new TextEditorTextSpan(
                                codeBlockOrExpressionStartingPositionIndex,
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

    private static object ReadComment(
        StringWalker stringWalker, 
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag, 
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        // Razor Comment
        //
        // Example: @* This is a comment*@
    }
}