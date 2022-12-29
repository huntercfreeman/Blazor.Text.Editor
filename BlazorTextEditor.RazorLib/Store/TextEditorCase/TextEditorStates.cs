using System.Collections.Immutable;
using BlazorALaCarte.Shared.Facts;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(
    ImmutableList<TextEditorBase> TextEditorList,
    TextEditorOptions GlobalTextEditorOptions)
{
    public TextEditorStates()
        : this(ImmutableList<TextEditorBase>.Empty, new TextEditorOptions(
            20,
            ThemeFacts.VisualStudioDarkThemeClone,
            false,
            false,
            null,
            2.5,
            KeymapFacts.DefaultKeymapDefinition))
    {
    }
}