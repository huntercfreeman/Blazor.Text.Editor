using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorHeader : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public string? FileExtension { get; set; }
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }

    private TextEditorDisplay? _textEditorDisplay;
    private TextEditorBase? _textEditorBase;

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

    private Task DoCopyOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoCutOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoPasteOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoRedoOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoSaveOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoUndoOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private Task DoSelectAllOnClick(MouseEventArgs arg)
    {
        throw new NotImplementedException();
    }
}