using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Razor.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;

public class TextEditorRazorLexer : ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            text,
            RazorInjectedLanguageFacts.RazorInjectedLanguageDefinition);

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