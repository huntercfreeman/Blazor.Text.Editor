using BlazorTextEditor.RazorLib.Model;

namespace BlazorTextEditor.Demo.RazorLib;

public class DemoStateContainer : IDemoStateContainer
{
    public WellKnownModelKind SelectedWellKnownModelKind { get; set; }
    public string InputResourceUri { get; set; } = string.Empty;
    public bool InputInitializeWithText { get; set; }
}