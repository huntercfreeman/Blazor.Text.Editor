using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.HelperComponents;

/// <summary>
/// The parent component of the text
/// editor helper component
/// does not need to re-render.
/// </summary>
public record TextEditorHelperComponentParameters(
    TextEditorDisplay? TextEditorDisplay,
    TextEditorBase? TextEditorBase);