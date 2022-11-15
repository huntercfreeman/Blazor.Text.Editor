using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Autocomplete;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_textEditorAutocompleteMenuElementReference is not null)
            {
                try
                {
                    await _textEditorAutocompleteMenuElementReference.Value
                        .FocusAsync();
                }
                catch (JSException)
                {
                    // Caused when calling:
                    // await (ElementReference).FocusAsync();
                    // After component is no longer rendered
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None, true);
    }

    private MenuRecord GetMenuRecord()
    {
        var autocompleteOptions = AutocompleteService
            .GetAutocompleteOptions(string.Empty);

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
                "No Autocomplete Results",
                MenuOptionKind.Other));
        }

        return new MenuRecord(
            menuOptionRecords
                .ToImmutableArray());
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