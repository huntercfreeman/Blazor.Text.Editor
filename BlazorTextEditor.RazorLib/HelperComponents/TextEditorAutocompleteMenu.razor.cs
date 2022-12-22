using System.Collections.Immutable;
using BlazorALaCarte.Shared.Keyboard;
using BlazorALaCarte.Shared.Menu;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
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
    [CascadingParameter(Name="TextEditorMenuShouldTakeFocusFunc")]
    public Func<bool> TextEditorMenuShouldTakeFocusFunc { get; set; } = null!;

    private ElementReference? _textEditorAutocompleteMenuElementReference;
    private MenuDisplay? _autocompleteMenuDisplay;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (TextEditorMenuShouldTakeFocusFunc.Invoke())
        {
            _autocompleteMenuDisplay?.SetFocusToFirstOptionInMenu();
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
    }
    
    private void ReturnFocusToThis()
    {
        _ = Task.Run(async () =>
        {
            await SetShouldDisplayMenuAsync
                .Invoke(TextEditorMenuKind.None, true);
        });
    }

    private MenuRecord GetMenuRecord()
    {
        var textEditor = TextEditorStatesSelection.Value;
        var localTextEditorViewModel = ReplaceableTextEditorViewModel;

        var cursorSnapshots =
            TextEditorCursorSnapshot.TakeSnapshots(
                localTextEditorViewModel?.PrimaryCursor ?? new TextEditorCursor(true));

        var primaryCursorSnapshot = cursorSnapshots
            .First(x => x.UserCursor.IsPrimaryCursor);

        if (textEditor is not null &&
            localTextEditorViewModel is not null &&
            primaryCursorSnapshot.ImmutableCursor.ColumnIndex > 0)
        {
            var word = textEditor.ReadPreviousWordOrDefault(
                primaryCursorSnapshot.ImmutableCursor.RowIndex,
                primaryCursorSnapshot.ImmutableCursor.ColumnIndex);

            List<MenuOptionRecord> menuOptionRecords = new();
            
            if (word is not null)
            {
                var autocompleteOptions = AutocompleteService
                    .GetAutocompleteOptions(word);

                menuOptionRecords = autocompleteOptions
                    .Select(option => new MenuOptionRecord(
                        option,
                        MenuOptionKind.Other,
                        () =>
                            SelectMenuOption(() =>
                                InsertAutocompleteMenuOption(
                                    word, 
                                    option,
                                    localTextEditorViewModel))))
                    .ToList();
            }
            
            if (!menuOptionRecords.Any())
            {
                menuOptionRecords.Add(new MenuOptionRecord(
                    "No results",
                    MenuOptionKind.Other));
            }

            return new MenuRecord(
                menuOptionRecords
                    .ToImmutableArray());
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

    private async Task InsertAutocompleteMenuOption(
        string word,
        string option,
        TextEditorViewModel textEditorViewModel)
    {
        var insertTextTextEditorBaseAction = new InsertTextTextEditorBaseAction(
            textEditorViewModel.TextEditorKey,
            TextEditorCursorSnapshot.TakeSnapshots(textEditorViewModel.PrimaryCursor),
            option.Substring(word.Length),
            CancellationToken.None);

        TextEditorService.InsertText(insertTextTextEditorBaseAction);
    }
}