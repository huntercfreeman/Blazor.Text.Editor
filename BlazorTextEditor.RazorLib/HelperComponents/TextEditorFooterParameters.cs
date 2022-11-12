using BlazorTextEditor.RazorLib.TextEditor;

namespace BlazorTextEditor.RazorLib.HelperComponents;

/// <summary>
/// The parent component of the TextEditorFooter
/// does not need to re-render.
/// </summary>
public record TextEditorFooterParameters(
    TextEditorDisplay? TextEditorDisplay,
    TextEditorBase? TextEditorBase);