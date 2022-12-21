using System.Collections.Immutable;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands;

public static class TextEditorCommandFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        _ => Task.CompletedTask,
        false,
        "DoNothingDiscard",
        "defaults_do-nothing-discard");
    
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
                
                await textEditorCommandParameter.TextEditorViewModel.FocusTextEditorAsync();
            }
        },
        false,
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

                await textEditorCommandParameter.TextEditorViewModel.FocusTextEditorAsync();
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
        },
        true,
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
        },
        true,
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");

    public static readonly TextEditorCommand Save = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .TextEditorViewModel
                .OnSaveRequested?
                .Invoke(
                    textEditorCommandParameter.TextEditor);
            
            textEditorCommandParameter.TextEditorService
                .ForceRerender(textEditorCommandParameter.TextEditor.Key);
            
            return Task.CompletedTask;
        },
        false,
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
            
            textEditorCommandParameter.TextEditorService
                .ForceRerender(textEditorCommandParameter.TextEditor.Key);
            
            return Task.CompletedTask;
        },
        false,
        "Select All",
        "defaults_select-all");
    
    public static readonly TextEditorCommand Undo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorService
                .UndoEdit(textEditorCommandParameter.TextEditor.Key);
            
            return Task.CompletedTask;
        },
        true,
        "Undo",
        "defaults_undo");
    
    public static readonly TextEditorCommand Redo = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorService
                .RedoEdit(textEditorCommandParameter.TextEditor.Key);

            return Task.CompletedTask;
        },
        true,
        "Redo",
        "defaults_redo");
    
    public static readonly TextEditorCommand Remeasure = new(textEditorCommandParameter =>
        {
            textEditorCommandParameter.TextEditorViewModel.ShouldMeasureDimensions = true;
            
            textEditorCommandParameter.TextEditorService
                .ForceRerender(textEditorCommandParameter.TextEditor.Key);
            
            return Task.CompletedTask;
        },
        false,
        "Remeasure",
        "defaults_remeasure");
    
    public static readonly TextEditorCommand ScrollLineDown = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByLinesAsync(1);
        },
        false,
        "Scroll Line Down",
        "defaults_scroll-line-down");
    
    public static readonly TextEditorCommand ScrollLineUp = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByLinesAsync(-1);
        },
        false,
        "Scroll Line Up",
        "defaults_scroll-line-up");
    
    public static readonly TextEditorCommand ScrollPageDown = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByPagesAsync(1);
        },
        false,
        "Scroll Page Down",
        "defaults_scroll-page-down");
    
    public static readonly TextEditorCommand ScrollPageUp = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .MutateScrollVerticalPositionByPagesAsync(-1);
        },
        false,
        "Scroll Page Up",
        "defaults_scroll-page-up");
    
    public static readonly TextEditorCommand CursorMovePageBottom = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .CursorMovePageBottomAsync();
        },
        false,
        "Move Cursor to Bottom of the Page",
        "defaults_cursor-move-page-bottom");
    
    public static readonly TextEditorCommand CursorMovePageTop = new(async textEditorCommandParameter =>
        {
            await textEditorCommandParameter.TextEditorViewModel
                .CursorMovePageTopAsync();
        },
        false,
        "Move Cursor to Top of the Page",
        "defaults_cursor-move-page-top");
}