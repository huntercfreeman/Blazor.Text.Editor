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
        var textEditor = TextEditorBase;
        
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

        if (TextEditorCursor.ShouldRevealCursor)
        {
            TextEditorCursor.ShouldRevealCursor = false;
        
            // Do not block UI thread
            _ = Task.Run(async () =>
            {
                await ScrollIntoViewIfNotVisibleAsync();
            });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetCursorStyleCss()
    {
        var textEditor = TextEditorBase;

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

            leftInPixels += extraWidthPerTabKey * 
                            tabsOnSameRowBeforeCursor * 
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        // Line number column offset
        {
            var mostDigitsInARowLineNumber = textEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * 
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;

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

        var width = $"width: {textEditor.MostCharactersOnASingleRow * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels}px;";

        return $"{top} {width} {height}";
    }

    private string GetMenuStyleCss()
    {
        var textEditor = TextEditorBase;
        
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
                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        // Line number column offset
        {
            var mostDigitsInARowLineNumber = textEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
        }

        leftInPixels += TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;

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
                var textEditor = TextEditorBase;
                var textEditorViewModel = TextEditorViewModel;

                var textEditorCursorSnapshot = new TextEditorCursorSnapshot(TextEditorCursor);
                
                if (textEditorViewModel.VirtualizationResult.Entries.Any())
                {
                    var firstEntry = textEditorViewModel.VirtualizationResult.Entries.First();
                    var lastEntry = textEditorViewModel.VirtualizationResult.Entries.Last();
                    
                    var lowerRowBoundInclusive = firstEntry.Index;
                    var upperRowBoundExclusive = lastEntry.Index + 1;

                    // Set scroll margin for determining if a row is out of view
                    {
                        lowerRowBoundInclusive += 1;
                        upperRowBoundExclusive -= 1;
                    }
                    
                    double? setScrollTopTo = null;

                    // Row is out of view
                    {
                        int? scrollToRowIndex = null;
                                                
                        if (textEditorCursorSnapshot.ImmutableCursor.RowIndex < lowerRowBoundInclusive)
                        {
                            scrollToRowIndex = textEditorCursorSnapshot.ImmutableCursor.RowIndex -
                                               WHEN_ROW_OUT_OF_VIEW_OVERSCROLL_BY;
                        }
                        else if(textEditorCursorSnapshot.ImmutableCursor.RowIndex >= upperRowBoundExclusive)
                        {
                            scrollToRowIndex = textEditorCursorSnapshot.ImmutableCursor.RowIndex -
                                               WHEN_ROW_OUT_OF_VIEW_OVERSCROLL_BY;
                        }

                        if (scrollToRowIndex is not null)
                        {
                            setScrollTopTo = scrollToRowIndex.Value * TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels;
                        }
                    }
                    
                    double? setScrollLeftTo = null;
                    
                    // Column is out of view
                    {
                        var lowerColumnPixelInclusive = textEditorViewModel
                            .VirtualizationResult.ElementMeasurementsInPixels.ScrollLeft;

                        var upperColumnPixelExclusive =
                            lowerColumnPixelInclusive + TextEditorViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width + 
                            1;

                        var leftInPixels = 0d;
                        
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
                                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
                        }
                        
                        // Account for cursor column index
                        {
                            leftInPixels += textEditorCursorSnapshot.ImmutableCursor.ColumnIndex * 
                                            TextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;
                        }
                        
                        if (leftInPixels < lowerColumnPixelInclusive ||
                            leftInPixels >= upperColumnPixelExclusive)
                        {
                            setScrollLeftTo = leftInPixels;
                        }
                    }

                    if (setScrollLeftTo is not null || 
                        setScrollTopTo is not null)
                    {
                        await textEditorViewModel.SetScrollPositionAsync(
                            setScrollLeftTo,
                            setScrollTopTo);

                        await Task.Delay(_checkCursorIsInViewDelay);
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
    
    public void Dispose()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
        _checkCursorIsInViewCancellationTokenSource.Cancel();
    }
}