using System.Collections.Immutable;
using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Keyboard;
using BlazorALaCarte.Shared.Menu;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorContextMenu : TextEditorView
{
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, bool, Task> SetShouldDisplayMenuAsync { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;

    private ElementReference? _textEditorContextMenuElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_textEditorContextMenuElementReference is not null)
            {
                try
                {
                    await _textEditorContextMenuElementReference.Value
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
    
    private TextEditorCommandParameter ConstructTextEditorCommandParameter(
        TextEditorBase textEditorBase,
        TextEditorDisplay textEditorDisplay)
    {
        return new TextEditorCommandParameter(
            textEditorBase,
            TextEditorCursorSnapshot.TakeSnapshots(textEditorDisplay.PrimaryCursor),
            ClipboardProvider,
            TextEditorService,
            textEditorDisplay);
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
        List<MenuOptionRecord> menuOptionRecords = new();

        var cut = new MenuOptionRecord(
            "Cut",
            MenuOptionKind.Other,
            () => SelectMenuOption(CutMenuOption));
        
        menuOptionRecords.Add(cut);

        var copy = new MenuOptionRecord(
            "Copy",
            MenuOptionKind.Other,
            () => SelectMenuOption(CopyMenuOption));

        menuOptionRecords.Add(copy);

        var paste = new MenuOptionRecord(
            "Paste",
            MenuOptionKind.Other,
            () => SelectMenuOption(PasteMenuOption));

        menuOptionRecords.Add(paste);

        if (!menuOptionRecords.Any())
        {
            menuOptionRecords.Add(new MenuOptionRecord(
                "No Context Menu Options for this item",
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

    private async Task CutMenuOption()
    {
        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Cut;
    
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
    
    private async Task CopyMenuOption()
    {
        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Copy;
    
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task PasteMenuOption()
    {
        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Paste;
    
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
}