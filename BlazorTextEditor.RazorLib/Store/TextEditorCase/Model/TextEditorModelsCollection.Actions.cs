using System.Collections.Immutable;
using BlazorALaCarte.Shared.Theme;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Model;

public partial class TextEditorModelsCollection
{
    public record DeleteTextByMotionTextEditorModelAction(
        TextEditorModelKey TextEditorModelKey,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
        MotionKind MotionKind,
        CancellationToken CancellationToken);

    public record DeleteTextByRangeTextEditorModelAction(
        TextEditorModelKey TextEditorModelKey,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
        int Count,
        CancellationToken CancellationToken);

    public record DisposeTextEditorModelAction(TextEditorModelKey TextEditorModelKey);

    public record ForceRerenderAction(TextEditorModelKey TextEditorModelKey);

    public record InsertTextTextEditorModelAction(
        TextEditorModelKey TextEditorModelKey,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
        string Content,
        CancellationToken CancellationToken);

    public record KeyboardEventTextEditorModelAction(
        TextEditorModelKey TextEditorModelKey,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshots,
        KeyboardEventArgs KeyboardEventArgs,
        CancellationToken CancellationToken);

    public record RedoEditAction(TextEditorModelKey TextEditorModelKey);

    public record RegisterTextEditorModelAction(TextEditorModel TextEditorModel);

    public record ReloadTextEditorModelAction(TextEditorModelKey TextEditorModelKey, string Content);

    public record TextEditorSetCursorWidthAction(double CursorWidthInPixels);

    public record TextEditorSetFontSizeAction(int FontSizeInPixels);

    public record TextEditorSetHeightAction(int? HeightInPixels);

    public record TextEditorSetKeymapAction(KeymapDefinition KeymapDefinition);

    public record TextEditorSetResourceDataAction(
        TextEditorModelKey TextEditorModelKey,
        string ResourceUri,
        DateTime ResourceLastWriteTime);

    public record TextEditorSetShowNewlinesAction(bool ShowNewlines);

    public record TextEditorSetShowWhitespaceAction(bool ShowWhitespace);

    public record TextEditorSetThemeAction(ThemeRecord Theme);

    public record TextEditorSetUsingRowEndingKindAction(
        TextEditorModelKey TextEditorModelKey,
        RowEndingKind RowEndingKind);

    public record UndoEditAction(TextEditorModelKey TextEditorModelKey);
}