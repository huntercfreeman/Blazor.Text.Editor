using System.Collections.Immutable;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;

public record TextEditorViewModel(
    TextEditorViewModelKey TextEditorViewModelKey,
    TextEditorKey TextEditorKey,
    ITextEditorService TextEditorService)
{
    // TODO: Tracking the most recently rendered virtualization result feels hacky and needs to be looked into further. The need for this arose when implementing the method "CursorMovePageBottomAsync()"
    public VirtualizationResult<List<RichCharacter>>? MostRecentlyRenderedVirtualizationResult { get; set; }
    
    public TextEditorCursor PrimaryCursor { get; } = new(true);
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();

    public string BodyElementId => $"bte_text-editor-content_{TextEditorViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"bte_text-editor-content_{TextEditorViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"bte_text-editor-gutter_{TextEditorViewModelKey.Guid}";

    public bool ShouldMeasureDimensions { get; set; } = true;
    public Action<TextEditorBase>? OnSaveRequested { get; set; }
    public CharacterWidthAndRowHeight? CharacterWidthAndRowHeight { get; set; }
    public WidthAndHeightOfTextEditor? WidthAndHeightOfBody { get; set; }

    public async Task CursorMovePageTopAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = MostRecentlyRenderedVirtualizationResult;

        if (localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false)
        {
            var firstEntry = localMostRecentlyRenderedVirtualizationResult.Entries.First();

            PrimaryCursor.IndexCoordinates = (firstEntry.Index, 0);
        }
    }

    public async Task CursorMovePageBottomAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = MostRecentlyRenderedVirtualizationResult;
        var textEditor = TextEditorService.GetTextEditorBaseFromViewModelKey(
            TextEditorViewModelKey);

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
            pages * (WidthAndHeightOfBody?.HeightInPixels ?? 0));
    }

    public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            lines * (CharacterWidthAndRowHeight?.RowHeightInPixels ?? 0));
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
    
    private VirtualizationResult<List<RichCharacter>>? EntriesProvider(
        VirtualizationRequest request)
    {
        var textEditorBase = TextEditorService.GetTextEditorBaseFromViewModelKey(TextEditorViewModelKey);
        
        if (CharacterWidthAndRowHeight is null ||
            WidthAndHeightOfBody is null ||
            textEditorBase is null ||
            request.CancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var verticalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollTopInPixels /
            CharacterWidthAndRowHeight.RowHeightInPixels);

        var verticalTake = (int)Math.Ceiling(
            WidthAndHeightOfBody.HeightInPixels /
            CharacterWidthAndRowHeight.RowHeightInPixels);
        
        // Vertical Padding (render some offscreen data)
        {
            verticalTake += 1;
        }
        
        // Check index boundaries
        {
            verticalStartingIndex = Math.Max(0, verticalStartingIndex);
            
            
            if (verticalStartingIndex + verticalTake >
                textEditorBase.RowEndingPositions.Length)
            {
                verticalTake = textEditorBase.RowEndingPositions.Length -
                               verticalStartingIndex;
            }
            
            verticalTake = Math.Max(0, verticalTake);
        }

        var horizontalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollLeftInPixels /
            CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var horizontalTake = (int)Math.Ceiling(
            WidthAndHeightOfBody.WidthInPixels /
            CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var virtualizedEntries = textEditorBase
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

                    var tabsOnSameRowBeforeCursor = textEditorBase
                        .GetTabsCountOnSameRowBeforeCursor(
                            index,
                            parameterForGetTabsCountOnSameRowBeforeCursor);

                    // 1 of the character width is already accounted for
                    var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

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
                    CharacterWidthAndRowHeight.CharacterWidthInPixels;

                var leftInPixels =
                    // do not change this to localHorizontalStartingIndex
                    horizontalStartingIndex *
                    CharacterWidthAndRowHeight.CharacterWidthInPixels;

                var topInPixels =
                    index *
                    CharacterWidthAndRowHeight.RowHeightInPixels;

                return new VirtualizationEntry<List<RichCharacter>>(
                    index,
                    horizontallyVirtualizedRow,
                    widthInPixels,
                    CharacterWidthAndRowHeight.RowHeightInPixels,
                    leftInPixels,
                    topInPixels);
            }).ToImmutableArray();

        var totalWidth =
            textEditorBase.MostCharactersOnASingleRow *
            CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var totalHeight =
            textEditorBase.RowEndingPositions.Length *
            CharacterWidthAndRowHeight.RowHeightInPixels;
        
        // Add vertical margin so the user can scroll beyond the final row of content
        {
            var percentOfMarginScrollHeightByPageUnit = 0.4;
            
            var marginScrollHeight =
                (WidthAndHeightOfBody?.HeightInPixels ?? 0) *
                percentOfMarginScrollHeightByPageUnit;

            totalHeight += marginScrollHeight;
        }

        var leftBoundaryWidthInPixels =
            horizontalStartingIndex *
            CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var leftBoundary = new VirtualizationBoundary(
            leftBoundaryWidthInPixels,
            null,
            0,
            0);

        var rightBoundaryLeftInPixels =
            leftBoundary.WidthInPixels +
            CharacterWidthAndRowHeight.CharacterWidthInPixels *
            horizontalTake;

        var rightBoundaryWidthInPixels =
            totalWidth -
            rightBoundaryLeftInPixels;

        var rightBoundary = new VirtualizationBoundary(
            rightBoundaryWidthInPixels,
            null,
            rightBoundaryLeftInPixels,
            0);

        var topBoundaryHeightInPixels =
            verticalStartingIndex *
            CharacterWidthAndRowHeight.RowHeightInPixels;

        var topBoundary = new VirtualizationBoundary(
            null,
            topBoundaryHeightInPixels,
            0,
            0);

        var bottomBoundaryTopInPixels =
            topBoundary.HeightInPixels +
            CharacterWidthAndRowHeight.RowHeightInPixels *
            verticalTake;

        var bottomBoundaryHeightInPixels =
            totalHeight -
            bottomBoundaryTopInPixels;

        var bottomBoundary = new VirtualizationBoundary(
            null,
            bottomBoundaryHeightInPixels,
            0,
            bottomBoundaryTopInPixels);
        
        return new VirtualizationResult<List<RichCharacter>>(
            virtualizedEntries,
            leftBoundary,
            rightBoundary,
            topBoundary,
            bottomBoundary,
            request.ScrollPosition with
            {
                ScrollWidthInPixels = totalWidth,
                ScrollHeightInPixels = totalHeight
            });
    }
}