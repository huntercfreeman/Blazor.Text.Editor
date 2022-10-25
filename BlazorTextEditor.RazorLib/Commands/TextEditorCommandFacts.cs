using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands;

public static class TextEditorCommandFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new TextEditorCommand(textEditorCommandParameter => { return Task.CompletedTask; },
        "Do Nothing",
        "defaults_do-nothing");
    
    public static readonly TextEditorCommand Copy = new TextEditorCommand(
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
            }
        },
        "Copy",
        "defaults_copy");
    
    public static readonly TextEditorCommand Paste = new TextEditorCommand(
        async textEditorCommandParameter =>
        {
            var primaryCursor = textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor;
            
            var clipboard = await textEditorCommandParameter
                .ClipboardProvider
                .ReadClipboard();

            var previousCharacterWasCarriageReturn = false;
    
            foreach (var character in clipboard)
            {
                if (previousCharacterWasCarriageReturn &&
                    character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
                {
                    previousCharacterWasCarriageReturn = false;
                    continue;
                }

                // Need innerCursorSnapshots because need
                // after every loop of the foreach that the
                // cursor snapshots are updated
                var innerCursorSnapshots = new TextEditorCursorSnapshot[]
                {
                    new TextEditorCursorSnapshot(
                        primaryCursor)
                }.ToImmutableArray();
        
                var code = character switch
                {
                    '\r' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                    '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                    '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                    ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                    _ => character.ToString()
                };

                textEditorCommandParameter.TextEditorService
                    .EditTextEditor(
                        new EditTextEditorBaseAction(
                            textEditorCommandParameter.TextEditor.Key,
                            innerCursorSnapshots,
                            new KeyboardEventArgs
                            {
                                Code = code,
                                Key = character.ToString()
                            },
                            CancellationToken.None));

                previousCharacterWasCarriageReturn = 
                    KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN 
                    == character;
            }

            textEditorCommandParameter
                .ReloadVirtualizationDisplay
                .Invoke();
        },
        "Paste",
        "defaults_paste",
        TextEditKind.Other,
        "defaults_paste");
    
    public static readonly TextEditorCommand Save = new TextEditorCommand(textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .OnSaveRequested?
                .Invoke(
                    textEditorCommandParameter.TextEditor);
            return Task.CompletedTask;
        },
        "Save",
        "defaults_save");
    
    public static readonly TextEditorCommand SelectAll = new TextEditorCommand(textEditorCommandParameter =>
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
}