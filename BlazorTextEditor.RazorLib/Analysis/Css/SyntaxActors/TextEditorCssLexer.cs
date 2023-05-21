using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;

public class TextEditorCssLexer : ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(text);
        
        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var cssSyntaxWalker = new CssSyntaxWalker();

        cssSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssIdentifierSyntaxes.Select(identifier =>
                identifier.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssCommentSyntaxes.Select(comment =>
                comment.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyNameSyntaxes.Select(propertyName =>
                propertyName.TextEditorTextSpan));
        
        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssPropertyValueSyntaxes.Select(propertyValue =>
                propertyValue.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}