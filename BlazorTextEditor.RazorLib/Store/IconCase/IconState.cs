using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.IconCase;

[FeatureState]
public record IconState(int IconSizeInPixels)
{
    public IconState() : this(18)
    {
        
    }
}