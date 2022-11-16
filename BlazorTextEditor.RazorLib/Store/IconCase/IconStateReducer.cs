using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.IconCase;

public class IconStateReducer
{
    [ReducerMethod]
    public static IconState ReduceSetIconSizeInPixelsAction(IconState previousIconState,
        SetIconSizeInPixelsAction setIconSizeInPixelsAction)
    {
        return previousIconState with
        {
            IconSizeInPixels = setIconSizeInPixelsAction.IconSizeInPixels
        };
    }
}