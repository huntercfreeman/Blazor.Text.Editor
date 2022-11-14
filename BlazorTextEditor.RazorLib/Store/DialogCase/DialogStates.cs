using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.DialogCase;

[FeatureState]
public record DialogStates(ImmutableList<DialogRecord> DialogRecords)
{
    public DialogStates() : this(ImmutableList<DialogRecord>.Empty)
    {
        
    }
}