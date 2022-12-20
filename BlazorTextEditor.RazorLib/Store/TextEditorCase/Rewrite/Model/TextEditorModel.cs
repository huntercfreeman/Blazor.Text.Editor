using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Model;

public record TextEditorModel
{
    public TextEditorModelKey TextEditorModelKey { get; set; } = TextEditorModelKey.NewTextEditorModelKey();
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; set; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
}