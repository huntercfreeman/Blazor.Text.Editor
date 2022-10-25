using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Commands;

public static class TextEditorCommandFacts
{
    public static readonly TextEditorCommand Copy = new TextEditorCommand(
        async textEditorCommandParameter =>
        {
            var result = textEditorCommandParameter
                .PrimaryCursorSnapshot
                .ImmutableCursor
                .ImmutableTextEditorSelection
                .GetSelectedText(
                    textEditorCommandParameter.TextEditor);

            if (result is not null)
            {
                await textEditorCommandParameter
                    .ClipboardProvider
                    .SetClipboard(
                        result);
            }
        },
        "Copy",
        "defaults_copy");
    
    public static readonly TextEditorCommand Paste = new TextEditorCommand(
        async textEditorCommandParameter =>
        {
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
                            textEditorCommandParameter.CursorSnapshots,
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
    
    public static readonly TextEditorCommand Save = new TextEditorCommand(
        async textEditorCommandParameter =>
        {
            textEditorCommandParameter
                .OnSaveRequested?
                .Invoke(
                    textEditorCommandParameter.TextEditor);
        },
        "Save",
        "defaults_save");
}