using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

public record TextEditorGroup
{
    public TextEditorGroupKey TextEditorGroupKey { get; set; } = TextEditorGroupKey.NewTextEditorGroupKey();
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; set; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}