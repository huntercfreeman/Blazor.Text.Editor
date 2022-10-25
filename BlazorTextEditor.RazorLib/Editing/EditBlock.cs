namespace BlazorTextEditor.RazorLib.MoveThese;

public record EditBlock(
    TextEditKind TextEditKind, 
    string DisplayName, 
    string ContentSnapshot,
    string? OtherTextEditKindIdentifier);