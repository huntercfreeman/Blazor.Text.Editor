using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands;

public static class TextEditorCommandFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        textEditorCommandParameter => { return Task.CompletedTask; },
        "Do Nothing",
        "defaults_do-nothing");

    public static readonly TextEditorCommand Copy = new(
        async textEditorCommandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection,
                    textEditorCommandParameter.TextEditor);

            if (selectedText is not null)
            {
                await textEditorCommandParameter
                    .ClipboardProvider
                    .SetClipboard(
                        selectedText);
                
                await textEditorCommandParameter.TextEditorDisplay.FocusTextEditorAsync();
            }
        },
        "Copy",
        "defaults_copy");
    
    public static readonly TextEditorCommand Cut = new(
        async textEditorCommandParameter =>
        {
            var selectedText = TextEditorSelectionHelper
                .GetSelectedText(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot
                        .ImmutableCursor
                        .ImmutableTextEditorSelection,
                    textEditorCommandParameter.TextEditor);

            if (selectedText is not null)
            {
                await textEditorCommandParameter
                    .ClipboardProvider
                    .SetClipboard(
                        selectedText);

                await textEditorCommandParameter.TextEditorDisplay.FocusTextEditorAsync();
            }
            
            textEditorCommandParameter.TextEditorService
                .HandleKeyboardEvent(new KeyboardEventTextEditorBaseAction(
                    textEditorCommandParameter.TextEditor.Key,
                    textEditorCommandParameter.CursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Key = KeyboardKeyFacts.MetaKeys.DELETE
                    },
                    CancellationToken.None));
            
            textEditorCommandParameter
                .TextEditorDisplay
                .ReloadVirtualizationDisplay();
        },
        "Cut",
        "defaults_cut");

    public static readonly TextEditorCommand Paste = new(
        async textEditorCommandParameter =>
        {
            var clipboard = await textEditorCommandParameter
                .ClipboardProvider
                .ReadClipboard();

            textEditorCommandParameter.TextEditorService.InsertText(
                new InsertTextTextEditorBaseAction(
                    textEditorCommandParameter.TextEditor.Key,
                    new[]
                    {
                        textEditorCommandParameter.PrimaryCursorSnapshot,
                    }.ToImmutableArray(),
                    clipboard,
                    CancellationToken.None));

            textEditorCommandParameter
                .TextEditorDisplay
                .ReloadVirtualizationDisplay();
        },
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly TextEditorCommand Save = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .TextEditorDisplay
                .OnSaveRequested?
                .Invoke(
                    textEditorCommandParameter.TextEditor);
            return Task.CompletedTask;
        },
        "Save",
        "defaults_save");

    public static readonly TextEditorCommand SelectAll = new(textEditorCommandParameter =>
        {
            var primaryCursor = textEditorCommandParameter
                .PrimaryCursorSnapshot.UserCursor;

            primaryCursor.TextEditorSelection.AnchorPositionIndex =
                0;

            primaryCursor.TextEditorSelection.EndingPositionIndex =
                textEditorCommandParameter.TextEditor.DocumentLength;
            return Task.CompletedTask;
        },
        "Select All",
        "defaults_select-all");
    
    public static readonly TextEditorCommand Undo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditor.UndoEdit();
            
            textEditorCommandParameter
                .TextEditorDisplay
                .ReloadVirtualizationDisplay();
            
            return Task.CompletedTask;
        },
        "Undo",
        "defaults_undo");
    
    public static readonly TextEditorCommand Redo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditor.RedoEdit();
            
            textEditorCommandParameter
                .TextEditorDisplay
                .ReloadVirtualizationDisplay();
            
            return Task.CompletedTask;
        },
        "Redo",
        "defaults_redo");
    
    public static readonly TextEditorCommand Remeasure = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorDisplay.ShouldMeasureDimensions = true;
            
            textEditorCommandParameter
                .TextEditorDisplay
                .ReloadVirtualizationDisplay();
            
            return Task.CompletedTask;
        },
        "Remeasure",
        "defaults_remeasure");
}