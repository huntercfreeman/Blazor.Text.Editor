using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Model;
using System.Collections.Immutable;
using System.Reflection;

namespace BlazorTextEditor.RazorLib.Lexing;

public interface ITextEditorLexer
{
    public RenderStateKey ModelRenderStateKey { get; }

    public Task<ImmutableArray<TextEditorTextSpan>> Lex(
        string text,
        RenderStateKey modelRenderStateKey);
}

public interface TextEditorLexerResult
{
    public RenderStateKey ModelRenderStateKey { get; }
    public ImmutableArray<TextEditorTextSpan> TextSpans { get; }
}