using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Menu;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorAutocompleteMenu : TextEditorView
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteService AutocompleteService { get; set; } = null!;

    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, bool, Task> SetShouldDisplayMenuAsync { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;

    private ElementReference? _textEditorAutocompleteMenuElementReference;

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
    }

    private MenuRecord GetMenuRecord()
    {
        var cursorSnapshots =
            TextEditorCursorSnapshot.TakeSnapshots(
                TextEditorDisplay.PrimaryCursor);

        var primaryCursorSnapshot = cursorSnapshots
            .First(x => x.UserCursor.IsPrimaryCursor);

        if (primaryCursorSnapshot.ImmutableCursor.ColumnIndex > 0)
        {
            var possibleWordColumnIndexEnd =
                primaryCursorSnapshot.ImmutableCursor.ColumnIndex -
                1;

            var positionIndex = TextEditor.GetPositionIndex(
                primaryCursorSnapshot.ImmutableCursor.RowIndex,
                possibleWordColumnIndexEnd);

            var characterKindBehindCurrent = TextEditor.GetCharacterKindAt(
                positionIndex);

            if (characterKindBehindCurrent == CharacterKind.LetterOrDigit)
            {
                var wordColumnIndexStart = TextEditor
                    .GetColumnIndexOfCharacterWithDifferingKind(
                        primaryCursorSnapshot.ImmutableCursor.RowIndex,
                        possibleWordColumnIndexEnd,
                        true);

                wordColumnIndexStart =
                    wordColumnIndexStart == -1
                        ? 0
                        : wordColumnIndexStart;

                var wordLength = possibleWordColumnIndexEnd -
                                 wordColumnIndexStart;

                var wordStartingPositionIndex =
                    possibleWordColumnIndexEnd - wordLength;

                var word = TextEditor.GetTextRange(
                    wordStartingPositionIndex,
                    wordLength
                    + 1);

                var autocompleteOptions = AutocompleteService
                    .GetAutocompleteOptions(word);

                List<MenuOptionRecord> menuOptionRecords = autocompleteOptions
                    .Select(option => new MenuOptionRecord(
                        option,
                        MenuOptionKind.Other,
                        () =>
                            SelectMenuOption(() =>
                                InsertAutocompleteMenuOption(option))))
                    .ToList();

                if (!menuOptionRecords.Any())
                {
                    menuOptionRecords.Add(new MenuOptionRecord(
                        word + ' ' + "No Autocomplete Results",
                        MenuOptionKind.Other));
                }

                return new MenuRecord(
                    menuOptionRecords
                        .ToImmutableArray());
            }
        }

        return new MenuRecord(
            new MenuOptionRecord[]
            {
                new(
                    "No results",
                    MenuOptionKind.Other)
            }.ToImmutableArray());
    }

    private void SelectMenuOption(Func<Task> menuOptionAction)
    {
        _ = Task.Run(async () =>
        {
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
            await menuOptionAction();
        });
    }

    private async Task InsertAutocompleteMenuOption(string option)
    {
        var insertTextTextEditorBaseAction = new InsertTextTextEditorBaseAction(
            TextEditor.Key,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorDisplay.PrimaryCursor),
            option,
            CancellationToken.None);

        TextEditorService.InsertText(insertTextTextEditorBaseAction);
    }
}