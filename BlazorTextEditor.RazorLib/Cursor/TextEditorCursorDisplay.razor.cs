using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Cursor;

public partial class TextEditorCursorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [CascadingParameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;
    [CascadingParameter]
    public TextEditorViewModel TextEditorViewModel { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ScrollableContainerId { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsFocusTarget { get; set; }
    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    [Parameter]
    public RenderFragment OnContextMenuRenderFragment { get; set; } = null!;
    [Parameter]
    public RenderFragment AutoCompleteMenuRenderFragment { get; set; } = null!;
    
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    private bool _hasBlinkAnimation = true;
    
    private CancellationTokenSource _checkCursorIsInViewCancellationTokenSource = new();
    private SemaphoreSlim _checkCursorIsInViewSemaphoreSlim = new(1, 1);
    private int _skippedCheckCursorIsInViewCount;
    private readonly TimeSpan _checkCursorIsInViewDelay = TimeSpan.FromMilliseconds(25);

    private ElementReference? _textEditorCursorDisplayElementReference;
    private TextEditorMenuKind _textEditorMenuKind;
    private int _textEditorMenuShouldGetFocusRequestCount;
    private bool _disposedValue;

    private string _previouslyObservedTextEditorCursorDisplayId = string.Empty;

    /// <summary>
    /// Scroll by 2 more rows than necessary to bring an out of view row into view.
    /// </summary>
    private const int WHEN_ROW_OUT_OF_VIEW_OVERSCROLL_BY = 2;
    
    /// <summary>
    /// Determine if a row is out of view with the lower and upper boundaries each being 1 row narrower.
    /// </summary>
    private const int SCROLL_MARGIN_FOR_ROW_OUT_OF_VIEW = 1;
    /// <summary>
    /// Determine if a column is out of view with the lower and upper boundaries each being 1 column narrower.
    /// </summary>
    private const int SCROLL_MARGIN_FOR_COLUMN_OUT_OF_VIEW = 1;

    public string TextEditorCursorDisplayId => TextEditorCursor.IsPrimaryCursor
        ? TextEditorViewModel.PrimaryCursorContentId
        : string.Empty;

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string MenuStyleCss => GetMenuStyleCss();
    public string BlinkAnimationCssClass => _hasBlinkAnimation
        ? "bte_blink"
        : string.Empty;
    
    public TextEditorMenuKind TextEditorMenuKind => _textEditorMenuKind;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_previouslyObservedTextEditorCursorDisplayId != TextEditorCursorDisplayId)
        {
            if (IsFocusTarget)
            {
                await JsRuntime.InvokeVoidAsync(
                    "blazorTextEditor.initializeTextEditorCursorIntersectionObserver",
                    _intersectionObserverMapKey.ToString(),
                    DotNetObjectReference.Create(this),
                    ScrollableContainerId,
                    TextEditorCursorDisplayId);
                
                _previouslyObservedTextEditorCursorDisplayId = TextEditorCursorDisplayId;
            }
        }
        
        var textEditor = TextEditorBase;
        
        var rowIndex = TextEditorCursor.IndexCoordinates.rowIndex;

        // Ensure cursor stays within the row count index range
        if (rowIndex > textEditor.RowCount - 1)
            rowIndex = textEditor.RowCount - 1;
        
        var columnIndex = TextEditorCursor.IndexCoordinates.columnIndex;

        var rowLength = textEditor.GetLengthOfRow(rowIndex);

        // Ensure cursor stays within the column count index range for the current row
        if (columnIndex > rowLength)
            columnIndex = rowLength - 1;

        rowIndex = Math.Max(0, rowIndex);
        columnIndex = Math.Max(0, columnIndex);
        
        TextEditorCursor.IndexCoordinates = (rowIndex, columnIndex);

        if (TextEditorCursor.ShouldRevealCursor)
        {
            TextEditorCursor.ShouldRevealCursor = false;

            if (!TextEditorCursor.IsIntersecting)
            {
                await JsRuntime.InvokeVoidAsync(
                    "blazorTextEditor.scrollElementIntoView",
                    TextEditorCursorDisplayId);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    [JSInvokable]
    public Task OnCursorPassedIntersectionThresholdAsync(bool cursorIsIntersecting)
    {
        TextEditorCursor.IsIntersecting = cursorIsIntersecting;
        return Task.CompletedTask;
    }

    private string GetCursorStyleCss()
    {
        var textEditor = TextEditorBase;

        var leftInPixels = 0d;

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = textEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex,
                    TextEditorCursor.IndexCoordinates.columnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey * 
                            tabsOnSameRowBeforeCursor * 
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels *
                        TextEditorCursor.IndexCoordinates.columnIndex;

        var left = $"left: {leftInPixels}px;";
        var top =
            $"top: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        return $"{left} {top} {height}";
    }

    private string GetCaretRowStyleCss()
    {
        var textEditor = TextEditorBase;

        var top =
            $"top: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels}px;";

        var widthOfBody = textEditor.MostCharactersOnASingleRowTuple.rowLength *
                          TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight
                              .CharacterWidthInPixels;
        
        var width = $"width: {widthOfBody}px;";

        return $"{top} {width} {height}";
    }

    private string GetMenuStyleCss()
    {
        var textEditor = TextEditorBase;
        
        var leftInPixels = 0d;

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = textEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex,
                    TextEditorCursor.IndexCoordinates.columnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels *
                        TextEditorCursor.IndexCoordinates.columnIndex;

        var left = $"left: {leftInPixels}px;";

        // Top is 1 row further than the cursor so it does not cover text at cursor position.
        var top =
            $"top: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels * (TextEditorCursor.IndexCoordinates.rowIndex + 1)}px;";

        var minWidth = $"min-Width: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels * 16}px;";
        var minHeight = $"min-height: {TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels * 4}px;";

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

        var cancellationToken = CancelBlinkingCursorSourceAndCreateNewThenReturnToken();

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

    private void HandleOnKeyDown()
    {
        PauseBlinkAnimation();
    }

    private CancellationToken CancelBlinkingCursorSourceAndCreateNewThenReturnToken()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
        _blinkingCursorCancellationTokenSource = new CancellationTokenSource();

        return _blinkingCursorCancellationTokenSource.Token;
    }
    
    private CancellationToken CancelCheckCursorInViewSourceAndCreateNewThenReturnToken()
    {
        _checkCursorIsInViewCancellationTokenSource.Cancel();
        _checkCursorIsInViewCancellationTokenSource = new CancellationTokenSource();

        return _checkCursorIsInViewCancellationTokenSource.Token;
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
    
    public void Dispose()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
        _checkCursorIsInViewCancellationTokenSource.Cancel();
        
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
}