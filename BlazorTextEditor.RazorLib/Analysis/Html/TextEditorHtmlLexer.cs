using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html;

public class TextEditorHtmlLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(text);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameSyntaxes
                .Select(tns => tns.TextEditorTextSpan));
        }

        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentSyntaxes
                .Select(ilfs => ilfs.TextEditorTextSpan));
        }

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}