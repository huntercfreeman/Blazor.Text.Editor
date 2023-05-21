using BlazorCommon.RazorLib.Misc;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Lexing;

public class TextEditorLexerDefault : ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; private set; } = RenderStateKey.Empty;

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey)
    {
        return Task.FromResult(ImmutableArray<TextEditorTextSpan>.Empty);
    }
}