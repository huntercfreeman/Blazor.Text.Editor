﻿using BlazorALaCarte.Shared.Theme;

namespace BlazorTextEditor.RazorLib.TextEditor;

/// <summary>
///     Any property on <see cref="TextEditorServiceOptions" /> will be equal to
///     the
/// </summary>
public record TextEditorOptions(
    int? FontSizeInPixels,
    ThemeRecord? Theme,
    bool? ShowWhitespace,
    bool? ShowNewlines,
    int? HeightInPixels,
    double? CursorWidthInPixels)
{
    public static TextEditorOptions UnsetTextEditorOptions()
    {
        return new TextEditorOptions(
            null,
            null,
            false,
            false,
            null,
            0);
    }
}