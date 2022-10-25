namespace BlazorTextEditor.RazorLib.TextEditor;

public record EditBlock(
    TextEditKind TextEditKind, 
    string DisplayName, 
    string ContentSnapshot,
    string? OtherTextEditKindIdentifier);