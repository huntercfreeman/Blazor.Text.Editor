using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(
    ImmutableList<TextEditorBase> TextEditorList,
    TextEditorOptions GlobalTextEditorOptions)
{
    public TextEditorStates() 
        : this(ImmutableList<TextEditorBase>.Empty, new TextEditorOptions())
    {
    }
}