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

    [Parameter]
    public string? FileExtension { get; set; }
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

    private TextEditorBase? _textEditorBase;
    private TextEditorDisplay? _textEditorDisplay;

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

    public async Task ReRenderTextEditorHeaderAsync(
        TextEditorHelperComponentParameters textEditorHelperComponentParameters)
    {
        _textEditorBase = textEditorHelperComponentParameters.TextEditorBase;
        _textEditorDisplay = textEditorHelperComponentParameters.TextEditorDisplay;

        await InvokeAsync(StateHasChanged);
    }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        if (_textEditorBase is null)
            return;

        var textEditorKey = _textEditorBase.Key;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(textEditorKey, rowEndingKind);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Copy;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Cut;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Paste;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Redo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Save;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Undo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.SelectAll;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRefreshOnClick(MouseEventArgs arg)
    {
        if (_textEditorBase is null || 
            _textEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            _textEditorBase,
            _textEditorDisplay);

        var command = TextEditorCommandFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
}