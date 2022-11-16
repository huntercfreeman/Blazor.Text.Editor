using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Cursor;

public partial class TextEditorCursorDisplay : TextEditorView
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ScrollableContainerId { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsFocusTarget { get; set; }
    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    [Parameter]
    public RenderFragment? OnContextMenuRenderFragment { get; set; }
    [Parameter]
    public RenderFragment? AutoCompleteMenuRenderFragment { get; set; }
    
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    private bool _hasBlinkAnimation = true;

    private ElementReference? _textEditorCursorDisplayElementReference;
    private TextEditorMenuKind _textEditorMenuKind;
    private int _textEditorMenuShouldGetFocusRequestCount;

    public string TextEditorCursorDisplayId => $"bte_text-editor-cursor-display_{_intersectionObserverMapKey}";

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string MenuStyleCss => GetMenuStyleCss();
    public string BlinkAnimationCssClass => _hasBlinkAnimation
        ? "bte_blink"
        : string.Empty;
    
    public TextEditorMenuKind TextEditorMenuKind => _textEditorMenuKind;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (IsFocusTarget)
            {
                await JsRuntime.InvokeVoidAsync(
                    "blazorTextEditor.initializeTextEditorCursorIntersectionObserver",
                    _intersectionObserverMapKey.ToString(),
                    ScrollableContainerId,
                    TextEditorCursorDisplayId);
            }
        }

        if (TextEditorCursor.ShouldRevealCursor)
        {
            TextEditorCursor.ShouldRevealCursor = false;
        
            await ScrollIntoViewIfNotVisibleAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetCursorStyleCss()
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null)
            return string.Empty;
        
        var leftInPixels = 0d;

        // Gutter padding column offset
        {
            leftInPixels +=
                TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;
        }

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = textEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex,
                    TextEditorCursor.IndexCoordinates.columnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                            CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        // Line number column offset
        {
            var mostDigitsInARowLineNumber = textEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += CharacterWidthAndRowHeight.CharacterWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;

        var left = $"left: {leftInPixels}px;";
        var top =
            $"top: {CharacterWidthAndRowHeight.RowHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        return $"{left} {top} {height}";
    }

    private string GetCaretRowStyleCss()
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null)
            return string.Empty;
        
        var top =
            $"top: {CharacterWidthAndRowHeight.RowHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var width = $"width: {textEditor.MostCharactersOnASingleRow * CharacterWidthAndRowHeight.CharacterWidthInPixels}px;";

        return $"{top} {width} {height}";
    }

    private string GetMenuStyleCss()
    {
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null)
            return string.Empty;
        
        var leftInPixels = 0d;

        // Gutter padding column offset
        {
            leftInPixels +=
                TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;
        }

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = textEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex,
                    TextEditorCursor.IndexCoordinates.columnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                            CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        // Line number column offset
        {
            var mostDigitsInARowLineNumber = textEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += CharacterWidthAndRowHeight.CharacterWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;

        var left = $"left: {leftInPixels}px;";

        // Top is 1 row further than the cursor so it does not cover text at cursor position.
        var top =
            $"top: {CharacterWidthAndRowHeight.RowHeightInPixels * (TextEditorCursor.IndexCoordinates.rowIndex + 1)}px;";

        var minWidth = $"min-Width: {CharacterWidthAndRowHeight.CharacterWidthInPixels * 16}px;";
        var minHeight = $"min-height: {CharacterWidthAndRowHeight.RowHeightInPixels * 4}px;";

        return $"{left} {top} {minWidth} {minHeight}";
    }

    public async Task FocusAsync()
    {
        if (_textEditorCursorDisplayElementReference is not null)
            await _textEditorCursorDisplayElementReference.Value.FocusAsync();
    }

    public void PauseBlinkAnimation()
    {
        _hasBlinkAnimation = false;

        var cancellationToken = CancelSourceAndCreateNewThenReturnToken();

        _ = Task.Run(async () =>
        {
            await Task.Delay(_blinkingCursorTaskDelay, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                _hasBlinkAnimation = true;
                await InvokeAsync(StateHasChanged);
            }
        }, cancellationToken);
    }

    public async Task ScrollIntoViewIfNotVisibleAsync()
    {
        if (_textEditorCursorDisplayElementReference is null)
            return;

        await JsRuntime.InvokeVoidAsync(
            "blazorTextEditor.revealCursor",
            _intersectionObserverMapKey.ToString(),
            TextEditorCursorDisplayId);
    }

    private void HandleOnKeyDown()
    {
        PauseBlinkAnimation();
    }

    private CancellationToken CancelSourceAndCreateNewThenReturnToken()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
        _blinkingCursorCancellationTokenSource = new CancellationTokenSource();

        return _blinkingCursorCancellationTokenSource.Token;
    }

    public async Task SetShouldDisplayMenuAsync(
        TextEditorMenuKind textEditorMenuKind,
        bool shouldFocusCursor = true)
    {
        // Clear the counter of requests for the Menu to take focus
        _ = TextEditorMenuShouldTakeFocus();
        
        _textEditorMenuKind = textEditorMenuKind;

        await InvokeAsync(StateHasChanged);

        if (shouldFocusCursor && _textEditorMenuKind == TextEditorMenuKind.None) 
            await FocusAsync();
    }

    public void SetFocusToActiveMenu()
    {
        _textEditorMenuShouldGetFocusRequestCount++;
        InvokeAsync(StateHasChanged);
    }

    private bool TextEditorMenuShouldTakeFocus()
    {
        if (_textEditorMenuShouldGetFocusRequestCount > 0)
        {
            _textEditorMenuShouldGetFocusRequestCount = 0;
            
            return true;
        }

        return false;
    }
    
    private int GetTabIndex()
    {
        if (!IsFocusTarget)
            return -1;

        return TabIndex;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _blinkingCursorCancellationTokenSource.Cancel();

            if (IsFocusTarget)
            {
                _ = Task.Run(async () =>
                {
                    await JsRuntime.InvokeVoidAsync(
                        "blazorTextEditor.disposeTextEditorCursorIntersectionObserver",
                        _intersectionObserverMapKey.ToString());
                });
            }
        }
        
        base.Dispose(true);
    }
}