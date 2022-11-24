using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
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
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    [Parameter, EditorRequired]
    public CharacterWidthAndRowHeight CharacterWidthAndRowHeight { get; set; } = null!;
    [Parameter, EditorRequired]
    public WidthAndHeightOfTextEditor WidthAndHeightOfTextEditor { get; set; } = null!;
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
    /// <summary>
    /// <see cref="GetMostRecentlyRenderedVirtualizationResultFunc"/> is a Func because
    /// the way <see cref="TextEditorDisplay"/> sets the <see cref="TextEditorDisplay._mostRecentlyRenderedVirtualizationResult"/>
    /// is through an interaction with the UserInterface that feels rather hacky.
    /// <br/><br/>
    /// Thereby a func will get the value once the value is updated by rendering the virtualization display
    /// and without having to render the cursor.
    /// </summary>
    [Parameter]
    public Func<VirtualizationResult<List<RichCharacter>>?> GetMostRecentlyRenderedVirtualizationResultFunc
    {
        get;
        set;
    } = null!;
    
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    private bool _hasBlinkAnimation = true;
    
    private CancellationTokenSource _checkCursorIsInViewCancellationTokenSource = new();
    private SemaphoreSlim _checkCursorIsInViewSemaphoreSlim = new(1, 1);
    private int _skippedCheckCursorIsInViewCount;

    private ElementReference? _textEditorCursorDisplayElementReference;
    private TextEditorMenuKind _textEditorMenuKind;
    private int _textEditorMenuShouldGetFocusRequestCount;

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
        var textEditor = TextEditorStatesSelection.Value;

        if (textEditor is null)
        {
            TextEditorCursor.IndexCoordinates = (0, 0);
        }
        else
        {
            var rowIndex = TextEditorCursor.IndexCoordinates.rowIndex;

            if (rowIndex > textEditor.RowCount - 1)
            {
                rowIndex = textEditor.RowCount - 1;
            }
            
            var columnIndex = TextEditorCursor.IndexCoordinates.columnIndex;

            var rowLength = textEditor.GetLengthOfRow(rowIndex);

            if (columnIndex > rowLength)
            {
                columnIndex = rowLength - 1;
            }

            rowIndex = Math.Max(0, rowIndex);
            columnIndex = Math.Max(0, columnIndex);
            
            TextEditorCursor.IndexCoordinates = (rowIndex, columnIndex);
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

    public async Task ScrollIntoViewIfNotVisibleAsync()
    {
        if (_textEditorCursorDisplayElementReference is null)
            return;
        
        var success = await _checkCursorIsInViewSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);

        if (!success)
        {
            _skippedCheckCursorIsInViewCount++;
            return;
        }

        try
        {
            do
            {
                var textEditor = TextEditorStatesSelection.Value;

                if (textEditor is null)
                    return;
                
                var mostRecentlyRenderedVirtualizationResult = GetMostRecentlyRenderedVirtualizationResultFunc
                    .Invoke();
                
                var textEditorCursorSnapshot = new TextEditorCursorSnapshot(TextEditorCursor);
                
                if (mostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false)
                {
                    var firstEntry = mostRecentlyRenderedVirtualizationResult.Entries.First();
                    var lastEntry = mostRecentlyRenderedVirtualizationResult.Entries.Last();
                    
                    var lowerRowBoundInclusive = firstEntry.Index;
                    var upperRowBoundExclusive = lastEntry.Index + 1;
                    
                    // Set scroll margin for determining if a row is out of view
                    {
                        lowerRowBoundInclusive += 1;
                        upperRowBoundExclusive -= 1;
                    }

                    // Row is out of view
                    if (textEditorCursorSnapshot.ImmutableCursor.RowIndex < lowerRowBoundInclusive ||
                        textEditorCursorSnapshot.ImmutableCursor.RowIndex >= upperRowBoundExclusive)
                    {
                        await JsRuntime.InvokeVoidAsync(
                            "blazorTextEditor.scrollElementIntoView",
                            _intersectionObserverMapKey.ToString(),
                            TextEditorCursorDisplayId);
                        
                        if (textEditorCursorSnapshot.ImmutableCursor.RowIndex < lowerRowBoundInclusive)
                        {
                            await TextEditorDisplay
                                .MutateScrollVerticalPositionByLinesAsync(
                                    -1 * WHEN_ROW_OUT_OF_VIEW_OVERSCROLL_BY);
                        }
                        else if(textEditorCursorSnapshot.ImmutableCursor.RowIndex >= upperRowBoundExclusive)
                        {
                            await TextEditorDisplay
                                .MutateScrollVerticalPositionByLinesAsync(
                                    WHEN_ROW_OUT_OF_VIEW_OVERSCROLL_BY);
                        }
                    }

                    /*
                     * TODO: fix bug: move cursor to end of a row which goes offscreen column wise. Then hit arrow up and the cursor will be erroneously placed in the margin if the row that was went to resulted in a lower enough column index as the gutter was not accounted for.
                     */
                    
                    // Column is out of view
                    {
                        var lowerColumnPixelInclusive = mostRecentlyRenderedVirtualizationResult
                            .VirtualizationScrollPosition.ScrollLeftInPixels;

                        var upperColumnPixelExclusive =
                            lowerColumnPixelInclusive + WidthAndHeightOfTextEditor.WidthInPixels + 
                            1;
                        
                        // TODO: I am not sure where I was going with this scroll margin logic. It is not doing what I expected it to do. Perhaps I'm thinking of virtualization having padding because sometimes the final row does not render due to minor decimal place inaccuracies making the virtualization component think it is out of view.
                        // // Set scroll margin for determining if a column is out of view
                        // {
                        //     var scrollMarginForColumnInPixels = SCROLL_MARGIN_FOR_COLUMN_OUT_OF_VIEW *
                        //                                      WidthAndHeightOfTextEditor.WidthInPixels;
                        //     
                        //     lowerColumnPixelInclusive += scrollMarginForColumnInPixels;
                        //     upperColumnPixelExclusive -= scrollMarginForColumnInPixels;
                        // }

                        var leftInPixels = 0d;

                        // Capture widthOfGutter outside of upcoming code block
                        // it is needed to displace scrollLeft after JavaScript scroll into view
                        double widthOfGutter;

                        // Account for Gutter Width
                        {
                            var mostDigitsInARowLineNumber = textEditor.RowCount
                                .ToString()
                                .Length;
                
                            var widthOfLargestLineNumber = mostDigitsInARowLineNumber *
                                                           CharacterWidthAndRowHeight.CharacterWidthInPixels;
                
                            var widthOfGutterPadding = TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                                                       TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

                            widthOfGutter = widthOfLargestLineNumber + widthOfGutterPadding;
                        
                            leftInPixels += widthOfGutter;
                        }
                        
                        // Account for Tab Key Width
                        {
                            var tabsOnSameRowBeforeCursor = textEditor
                                .GetTabsCountOnSameRowBeforeCursor(
                                    textEditorCursorSnapshot.ImmutableCursor.RowIndex,
                                    textEditorCursorSnapshot.ImmutableCursor.ColumnIndex);

                            // 1 of the tab's character width is already accounted for
                            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

                            leftInPixels += extraWidthPerTabKey * 
                                            tabsOnSameRowBeforeCursor *
                                            CharacterWidthAndRowHeight.CharacterWidthInPixels;
                        }
                        
                        // Account for cursor column index
                        {
                            leftInPixels += textEditorCursorSnapshot.ImmutableCursor.ColumnIndex * 
                                            CharacterWidthAndRowHeight.CharacterWidthInPixels;
                        }

                        if (leftInPixels < lowerColumnPixelInclusive)
                        {
                            await JsRuntime.InvokeVoidAsync(
                                "blazorTextEditor.scrollElementIntoView",
                                _intersectionObserverMapKey.ToString(),
                                TextEditorCursorDisplayId);

                            // Displace scrollLeft by width of the Gutter
                            await TextEditorDisplay
                                .MutateScrollHorizontalPositionByPixelsAsync(
                                    -1 * widthOfGutter);
                        }
                        else if(leftInPixels >= upperColumnPixelExclusive)
                        {
                            await JsRuntime.InvokeVoidAsync(
                                "blazorTextEditor.scrollElementIntoView",
                                _intersectionObserverMapKey.ToString(),
                                TextEditorCursorDisplayId);
                            
                            // Displace scrollLeft by width of the Gutter
                            await TextEditorDisplay
                                .MutateScrollHorizontalPositionByPixelsAsync(
                                    widthOfGutter);
                        }
                    }
                }
            } while (StartScrollIntoViewIfNotVisibleIfHasSkipped());
        }
        finally
        {
            _checkCursorIsInViewSemaphoreSlim.Release();
        }

        bool StartScrollIntoViewIfNotVisibleIfHasSkipped()
        {
            if (_skippedCheckCursorIsInViewCount > 0)
            {
                _skippedCheckCursorIsInViewCount = 0;

                return true;
            }

            return false;
        }
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
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _blinkingCursorCancellationTokenSource.Cancel();
            _checkCursorIsInViewCancellationTokenSource.Cancel();
        }
        
        base.Dispose(true);
    }
}