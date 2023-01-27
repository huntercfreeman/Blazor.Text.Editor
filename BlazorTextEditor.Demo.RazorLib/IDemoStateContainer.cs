using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.Demo.RazorLib;

public interface IDemoStateContainer
{
    public WellKnownModelKind SelectedWellKnownModelKind { get; set; }
    public string InputResourceUri { get; set; }
    public bool InputInitializeWithText { get; set; }
}