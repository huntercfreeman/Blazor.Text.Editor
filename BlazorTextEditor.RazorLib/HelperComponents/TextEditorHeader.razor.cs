using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorHeader : TextEditorView
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorDisplay? TextEditorDisplay { get; set; }
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
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(TextEditorKey, rowEndingKind);
    }

    private async Task DoCopyOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Copy;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoCutOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Cut;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoPasteOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;
        
        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Paste;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRedoOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;
        
        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Redo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSaveOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;
        
        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Save;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoUndoOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Undo;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.SelectAll;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    private async Task DoRemeasureOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }
    
    private async Task DoRefreshOnClick(MouseEventArgs arg)
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return;
        }

        var textEditorCommandParameter = ConstructTextEditorCommandParameter(
            textEditor,
            TextEditorDisplay);

        var command = TextEditorCommandFacts.Remeasure;
        
        await command.DoAsyncFunc.Invoke(
            textEditorCommandParameter);
    }

    /// <summary>
    /// disabled=@GetUndoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetUndoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetUndoDisabledAttribute()
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return true;
        }
        
        return !textEditor.CanUndoEdit();
    }
    
    /// <summary>
    /// disabled=@GetRedoDisabledAttribute()
    /// will toggle the attribute
    /// <br/><br/>
    /// disabled="@GetRedoDisabledAttribute()"
    /// will toggle the value of the attribute
    /// </summary>
    private bool GetRedoDisabledAttribute()
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null || 
            TextEditorDisplay is null)
        {
            return true;
        }
        
        return !textEditor.CanRedoEdit();
    }
}