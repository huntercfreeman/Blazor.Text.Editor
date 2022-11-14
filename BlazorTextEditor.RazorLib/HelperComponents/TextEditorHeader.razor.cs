using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorHeader : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorDisplay? TextEditorDisplay { get; set; }
    [Parameter, EditorRequired]
    public TextEditorBase? TextEditorBase { get; set; }
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

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

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        if (TextEditorBase is null)
            return;

        var textEditorKey = TextEditorBase.Key;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(textEditorKey, rowEndingKind);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Copy;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Cut;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Paste;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Redo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Save;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Undo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.SelectAll;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRefreshOnClick(MouseEventArgs arg)
    {
        if (TextEditorBase is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            TextEditorBase,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
}