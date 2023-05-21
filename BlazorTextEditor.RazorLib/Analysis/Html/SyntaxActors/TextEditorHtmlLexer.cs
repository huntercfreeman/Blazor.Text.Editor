using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

public class TextEditorHtmlLexer : ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(text);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameSyntaxes
                .Select(tn => tn.TextEditorTextSpan));
        }

        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentSyntaxes
                .Select(ilf => ilf.TextEditorTextSpan));
        }
        
        // Attribute Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeNameSyntaxes
                .Select(an => an.TextEditorTextSpan));
        }
        
        // Attribute Values
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeValueSyntaxes
                .Select(av => av.TextEditorTextSpan));
        }
        
        // Comments
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.CommentSyntaxes
                .Select(c => c.TextEditorTextSpan));
        }

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}