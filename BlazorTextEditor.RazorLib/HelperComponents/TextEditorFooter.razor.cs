using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorFooter : ComponentBase, IDisposable
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase?> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorDisplay? TextEditorDisplay { get; set; }
    [Parameter, EditorRequired]
    public string? FileExtension { get; set; }

    private TextEditorDisplay? _previousTextEditorDisplay;
    private TextEditorOptions? _previousGlobalTextEditorOptions;

    protected override Task OnParametersSetAsync()
    {
        if (TextEditorDisplay is null)
        {
            if (_previousTextEditorDisplay is not null)
                _previousTextEditorDisplay.CursorsChanged -= TextEditorDisplayOnCursorsChanged;
        }
        else if (_previousTextEditorDisplay is null ||
                 _previousTextEditorDisplay.GetHashCode() != TextEditorDisplay.GetHashCode())
        {
            if (_previousTextEditorDisplay is not null)
                _previousTextEditorDisplay.CursorsChanged -= TextEditorDisplayOnCursorsChanged;

            TextEditorDisplay.CursorsChanged += TextEditorDisplayOnCursorsChanged;
        }

        _previousTextEditorDisplay = TextEditorDisplay;

        return base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        TextEditorStatesWrap.StateChanged += TextEditorStatesWrapOnStateChanged;

        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorList
                .SingleOrDefault(x => x.Key == TextEditorKey));
        
        base.OnInitialized();
    }
    
    private void TextEditorStatesWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousGlobalTextEditorOptions is null ||
            _previousGlobalTextEditorOptions != TextEditorStatesWrap.Value.GlobalTextEditorOptions)
        {
            _previousGlobalTextEditorOptions = TextEditorStatesWrap.Value.GlobalTextEditorOptions;
            InvokeAsync(StateHasChanged);
        }
    }

    private void TextEditorDisplayOnCursorsChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void SelectRowEndingKindOnChange(ChangeEventArgs changeEventArgs)
    {
        var textEditorKey = TextEditorKey;

        var rowEndingKindString = (string)(changeEventArgs.Value ?? string.Empty);

        if (Enum.TryParse<RowEndingKind>(rowEndingKindString, out var rowEndingKind))
            TextEditorService.SetUsingRowEndingKind(textEditorKey, rowEndingKind);
    }
    
    public void Dispose()
    {
        if (TextEditorDisplay is not null)
            TextEditorDisplay.CursorsChanged -= TextEditorDisplayOnCursorsChanged;
        
        TextEditorStatesWrap.StateChanged -= TextEditorStatesWrapOnStateChanged;
    }
}