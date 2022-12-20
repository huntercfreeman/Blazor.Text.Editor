using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.View;

public record TextEditorView
{
    public TextEditorViewKey TextEditorViewKey { get; set; } = TextEditorViewKey.NewTextEditorViewKey();
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; set; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}