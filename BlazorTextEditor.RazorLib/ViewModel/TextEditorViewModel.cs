using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.Virtualization;

namespace BlazorTextEditor.RazorLib.ViewModel;

public record TextEditorViewModel(
    TextEditorViewModelKey ViewModelKey,
    TextEditorModelKey ModelKey,
    ITextEditorService TextEditorService,
    VirtualizationResult<List<RichCharacter>> VirtualizationResult,
    bool ShouldMeasureDimensions,
    bool DisplayCommandBar)
{
    private ElementMeasurementsInPixels _mostRecentBodyMeasurementsInPixels = new(0, 0, 0, 0, 0, 0, 0, CancellationToken.None);
    
    public TextEditorCursor PrimaryCursor { get; } = new(true);
    
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();
    public Action<TextEditorModel>? OnSaveRequested { get; init; }
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }

    public string CommandBarValue { get; set; } = string.Empty;
    
    public string BodyElementId => $"bte_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"bte_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"bte_text-editor-gutter_{ViewModelKey.Guid}";
    
    public void CursorMovePageTop()
    {
        var localMostRecentlyRenderedVirtualizationResult = VirtualizationResult;

        if (localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false)
        {
            var firstEntry = localMostRecentlyRenderedVirtualizationResult.Entries.First();

            PrimaryCursor.IndexCoordinates = (firstEntry.Index, 0);
        }
    }

    public void CursorMovePageBottom()
    {
        var localMostRecentlyRenderedVirtualizationResult = VirtualizationResult;
        var textEditor = TextEditorService.GetTextEditorModelFromViewModelKey(
            ViewModelKey);

        if (textEditor is not null &&
            (localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false))
        {
            var lastEntry = localMostRecentlyRenderedVirtualizationResult.Entries.Last();

            var lastEntriesRowLength = textEditor.GetLengthOfRow(lastEntry.Index);
            
            PrimaryCursor.IndexCoordinates = (lastEntry.Index, lastEntriesRowLength);
        }
    }
    
    public async Task MutateScrollHorizontalPositionByPixelsAsync(double pixels)
    {
        await TextEditorService.MutateScrollHorizontalPositionByPixelsAsync(
            BodyElementId,
            GutterElementId,
            pixels);
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        await TextEditorService.MutateScrollVerticalPositionByPixelsAsync(
            BodyElementId,
            GutterElementId,
            pixels);
    }

    public async Task MutateScrollVerticalPositionByPagesAsync(double pages)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            pages * _mostRecentBodyMeasurementsInPixels.Height);
    }

    public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            lines * VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels);
    }
    
    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public async Task SetScrollPositionAsync(double? scrollLeft, double? scrollTop)
    {
        await TextEditorService.SetScrollPositionAsync(
            BodyElementId,
            GutterElementId,
            scrollLeft,
            scrollTop);
    }

    public async Task FocusTextEditorAsync()
    {
        await TextEditorService.FocusPrimaryCursorAsync(
            PrimaryCursorContentId);
    }
    
    public async Task CalculateVirtualizationResultAsync(
        ElementMeasurementsInPixels? bodyMeasurementsInPixels,
        CancellationToken cancellationToken)
    {
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
        
        var localCharacterWidthAndRowHeight = VirtualizationResult.CharacterWidthAndRowHeight;
        
        var textEditorModel = TextEditorService.GetTextEditorModelFromViewModelKey(ViewModelKey);

        if (bodyMeasurementsInPixels is null)
        {
            bodyMeasurementsInPixels = await TextEditorService
                .GetElementMeasurementsInPixelsById(BodyElementId);
        }

        _mostRecentBodyMeasurementsInPixels = bodyMeasurementsInPixels; 

        bodyMeasurementsInPixels = bodyMeasurementsInPixels with
        {
            MeasurementsExpiredCancellationToken = cancellationToken 
        };
        
        if (textEditorModel is null ||
            bodyMeasurementsInPixels.MeasurementsExpiredCancellationToken.IsCancellationRequested)
        {
            return;
        }

        var verticalStartingIndex = (int)Math.Floor(
            bodyMeasurementsInPixels.ScrollTop /
            localCharacterWidthAndRowHeight.RowHeightInPixels);

        var verticalTake = (int)Math.Ceiling(
            bodyMeasurementsInPixels.Height /
            localCharacterWidthAndRowHeight.RowHeightInPixels);
        
        // Vertical Padding (render some offscreen data)
        {
            verticalTake += 1;
        }
        
        // Check index boundaries
        {
            verticalStartingIndex = Math.Max(0, verticalStartingIndex);
            
            
            if (verticalStartingIndex + verticalTake >
                textEditorModel.RowEndingPositions.Length)
            {
                verticalTake = textEditorModel.RowEndingPositions.Length -
                               verticalStartingIndex;
            }
            
            verticalTake = Math.Max(0, verticalTake);
        }

        var horizontalStartingIndex = (int)Math.Floor(
            bodyMeasurementsInPixels.ScrollLeft /
            localCharacterWidthAndRowHeight.CharacterWidthInPixels);

        var horizontalTake = (int)Math.Ceiling(
            bodyMeasurementsInPixels.Width /
            localCharacterWidthAndRowHeight.CharacterWidthInPixels);

        var virtualizedEntries = textEditorModel
            .GetRows(verticalStartingIndex, verticalTake)
            .Select((row, index) =>
            {
                index += verticalStartingIndex;
                
                var localHorizontalStartingIndex = horizontalStartingIndex;
                var localHorizontalTake = horizontalTake;

                // Adjust for tab key width
                {
                    var maxValidColumnIndex = row.Count - 1;
                    
                    var parameterForGetTabsCountOnSameRowBeforeCursor =
                        localHorizontalStartingIndex > maxValidColumnIndex
                            ? maxValidColumnIndex
                            : localHorizontalStartingIndex;

                    var tabsOnSameRowBeforeCursor = textEditorModel
                        .GetTabsCountOnSameRowBeforeCursor(
                            index,
                            parameterForGetTabsCountOnSameRowBeforeCursor);

                    // 1 of the character width is already accounted for
                    var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                    localHorizontalStartingIndex -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
                }

                if (localHorizontalStartingIndex + localHorizontalTake > row.Count)
                    localHorizontalTake = row.Count - localHorizontalStartingIndex;

                localHorizontalTake = Math.Max(0, localHorizontalTake);

                var horizontallyVirtualizedRow = row
                    .Skip(localHorizontalStartingIndex)
                    .Take(localHorizontalTake)
                    .ToList();

                var widthInPixels =
                    horizontallyVirtualizedRow.Count *
                    localCharacterWidthAndRowHeight.CharacterWidthInPixels;

                var leftInPixels =
                    // do not change this to localHorizontalStartingIndex
                    horizontalStartingIndex *
                    localCharacterWidthAndRowHeight.CharacterWidthInPixels;

                var topInPixels =
                    index *
                    localCharacterWidthAndRowHeight.RowHeightInPixels;

                return new VirtualizationEntry<List<RichCharacter>>(
                    index,
                    horizontallyVirtualizedRow,
                    widthInPixels,
                    localCharacterWidthAndRowHeight.RowHeightInPixels,
                    leftInPixels,
                    topInPixels);
            }).ToImmutableArray();

        var totalWidth =
            textEditorModel.MostCharactersOnASingleRowTuple.rowLength *
            localCharacterWidthAndRowHeight.CharacterWidthInPixels;

        var totalHeight =
            textEditorModel.RowEndingPositions.Length *
            localCharacterWidthAndRowHeight.RowHeightInPixels;
        
        // Add vertical margin so the user can scroll beyond the final row of content
        double marginScrollHeight;
        {
            var percentOfMarginScrollHeightByPageUnit = 0.4;
            
            marginScrollHeight = bodyMeasurementsInPixels.Height *
                                 percentOfMarginScrollHeightByPageUnit;

            totalHeight += marginScrollHeight;
        }

        var leftBoundaryWidthInPixels =
            horizontalStartingIndex *
            localCharacterWidthAndRowHeight.CharacterWidthInPixels;

        var leftBoundary = new VirtualizationBoundary(
            leftBoundaryWidthInPixels,
            totalHeight,
            0,
            0);

        var rightBoundaryLeftInPixels =
            leftBoundary.WidthInPixels +
            localCharacterWidthAndRowHeight.CharacterWidthInPixels *
            horizontalTake;

        var rightBoundaryWidthInPixels =
            totalWidth -
            rightBoundaryLeftInPixels;

        var rightBoundary = new VirtualizationBoundary(
            rightBoundaryWidthInPixels,
            totalHeight,
            rightBoundaryLeftInPixels,
            0);

        var topBoundaryHeightInPixels =
            verticalStartingIndex *
            localCharacterWidthAndRowHeight.RowHeightInPixels;

        var topBoundary = new VirtualizationBoundary(
            totalWidth,
            topBoundaryHeightInPixels,
            0,
            0);

        var bottomBoundaryTopInPixels =
            topBoundary.HeightInPixels +
            localCharacterWidthAndRowHeight.RowHeightInPixels *
            verticalTake;

        var bottomBoundaryHeightInPixels =
            totalHeight -
            bottomBoundaryTopInPixels;

        var bottomBoundary = new VirtualizationBoundary(
            totalWidth,
            bottomBoundaryHeightInPixels,
            0,
            bottomBoundaryTopInPixels);

        var virtualizationResult = new VirtualizationResult<List<RichCharacter>>(
            virtualizedEntries,
            leftBoundary,
            rightBoundary,
            topBoundary,
            bottomBoundary,
            bodyMeasurementsInPixels with
            {
                ScrollWidth = totalWidth,
                ScrollHeight = totalHeight,
                MarginScrollHeight = marginScrollHeight
            },
            localCharacterWidthAndRowHeight);
        
        TextEditorService.SetViewModelWith(
                ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    VirtualizationResult = virtualizationResult,
                    TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                });
    }
}