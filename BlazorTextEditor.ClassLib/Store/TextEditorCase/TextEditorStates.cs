using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.ClassLib.UniversalResourceIdentifier;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(ImmutableList<TextEditorBase> TextEditorList)
{
    public TextEditorStates() : this(ImmutableList<TextEditorBase>.Empty)
    {
    }
}