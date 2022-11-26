using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Css;

public class TextEditorCssLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var cssSyntaxUnit = CssSyntaxTree.ParseText(text);
        
        var syntaxNodeRoot = cssSyntaxUnit.CssDocumentSyntax;

        var cssSyntaxWalker = new CssSyntaxWalker();

        cssSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        textEditorTextSpans.AddRange(
            cssSyntaxWalker.CssCommentSyntaxes.Select(ccs =>
                ccs.TextEditorTextSpan));

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}