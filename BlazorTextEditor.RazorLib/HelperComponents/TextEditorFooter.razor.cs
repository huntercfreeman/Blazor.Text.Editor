﻿using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorFooter : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorBase? TextEditor { get; set; }
    [Parameter, EditorRequired]
    public TextEditorDisplay? TextEditorDisplay { get; set; }
    [Parameter, EditorRequired]
    public string? FileExtension { get; set; }

    private TextEditorDisplay? _previousTextEditorDisplay;

    protected override Task OnParametersSetAsync()
    {
        if (TextEditorDisplay is null)
        {
            if (_previousTextEditorDisplay is not null)
                _previousTextEditorDisplay.CursorsChanged -= PreviousTextEditorDisplayOnCursorsChanged;
        }
        else if (_previousTextEditorDisplay is null ||
                 _previousTextEditorDisplay.GetHashCode() != TextEditorDisplay.GetHashCode())
        {
            if (_previousTextEditorDisplay is not null)
                _previousTextEditorDisplay.CursorsChanged -= PreviousTextEditorDisplayOnCursorsChanged;

            TextEditorDisplay.CursorsChanged += PreviousTextEditorDisplayOnCursorsChanged;
        }

        _previousTextEditorDisplay = TextEditorDisplay;

        return base.OnParametersSetAsync();
    }

    private void PreviousTextEditorDisplayOnCursorsChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var textEditorKey = TextEditor?.Key;

        if (textEditorKey is null)
            return;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(textEditorKey, rowEndingKind);
    }
    
    public void Dispose()
    {
        if (TextEditorDisplay is not null)
            TextEditorDisplay.CursorsChanged -= PreviousTextEditorDisplayOnCursorsChanged;
    }
}