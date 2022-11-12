namespace BlazorTextEditor.RazorLib.Editing;

public enum TextEditKind
{
    None,
    InitialState,
    Other,
    Insertion,
    Deletion,
    ForcePersistEditBlock
}