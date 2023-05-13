using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Measurement;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Virtualization;
using BlazorTextEditor.RazorLib.Options;
using System.Reflection;
using BlazorCommon.RazorLib.JavaScriptObjects;
using Microsoft.JSInterop;
using BlazorCommon.RazorLib.Reactive;

namespace BlazorTextEditor.RazorLib.ViewModel;

/// <summary>Stores the state of the user interface.<br/><br/>For example, the user's <see cref="TextEditorCursor"/> instances are stored here.<br/><br/>Each <see cref="TextEditorViewModel"/> has a unique underlying <see cref="TextEditorModel"/>.<br/><br/>Therefore, if one has a <see cref="TextEditorModel"/> of a text file named "myHomework.txt", then arbitrary amount of <see cref="TextEditorViewModel"/>(s) can reference that <see cref="TextEditorModel"/>.<br/><br/>For example, maybe one has a main text editor, but also a peek window open of the same underlying <see cref="TextEditorModel"/>. The main text editor is one <see cref="TextEditorViewModel"/> and the peek window is a separate <see cref="TextEditorViewModel"/>. Both of those <see cref="TextEditorViewModel"/>(s) are referencing the same <see cref="TextEditorModel"/>. Therefore typing into the peek window will also result in the main text editor re-rendering with the updated text and vice versa.</summary>
public record TextEditorViewModel
{
    public TextEditorViewModel(
        TextEditorViewModelKey viewModelKey,
        TextEditorModelKey modelKey,
        ITextEditorService textEditorService,
        VirtualizationResult<List<RichCharacter>> virtualizationResult,
        bool shouldMeasureDimensions,
        bool shouldCalculateVirtualizationResult,
        bool displayCommandBar)
    {
        ViewModelKey = viewModelKey;
        ModelKey = modelKey;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
        DisplayCommandBar = displayCommandBar;
    }

    /// <summary>If a request to calculate the virtualization result occurs, but the text editor is currently being measured. Then, do not let the calculation occur until after the measurements are done.</summary>
    private readonly SemaphoreSlim GeneralOperationSemaphoreSlim = new(1, 1);

    private ElementMeasurementsInPixels _mostRecentBodyMeasurementsInPixels = new(0, 0, 0, 0, 0, 0, 0, CancellationToken.None);

    private readonly IThrottle<byte> _generalOperationThrottle = new Throttle<byte>(TimeSpan.FromMilliseconds(300));

    public TextEditorCursor PrimaryCursor { get; } = new(true);

    public TextEditorViewModelKey ViewModelKey { get; init; }
    public TextEditorModelKey ModelKey { get; init; }
    public ITextEditorService TextEditorService { get; init; }
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
    public bool DisplayCommandBar { get; init; }
    public Action<TextEditorModel>? OnSaveRequested { get; init; }
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }    
    /// <summary><see cref="FirstPresentationLayer"/> is painted prior to any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayer"/>.<br/><br/>When using the <see cref="FirstPresentationLayer"/> one might find their css overriden by for example, text being selected.</summary>
    public ImmutableList<TextEditorPresentationModel> FirstPresentationLayer { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    /// <summary><see cref="LastPresentationLayer"/> is painted after any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayer"/>.<br/><br/>When using the <see cref="LastPresentationLayer"/> one might find the selected text background not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayer"/>.</summary>
    public ImmutableList<TextEditorPresentationModel> LastPresentationLayer { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;

    /// <summary>If the <see cref="RenderStateKey"/> value changes, then the <see cref="TextEditorViewModel"/> needs to be re-rendered.</summary>
    public RenderStateKey RenderStateKey { get; set; } = RenderStateKey.Empty;
    /// <summary>If the <see cref="ModelRenderStateKey"/> value changes, then the <see cref="TextEditorViewModel"/> needs to have its <see cref="VirtualizationResult{T}"/> re-calculated.<br/><br/>The value is mutated in the Blazor 'OnAfterRender(...)' lifecycle method.</summary>
    public RenderStateKey ModelRenderStateKey { get; set; } = RenderStateKey.Empty;
    /// <summary>If the <see cref="OptionsRenderStateKey"/> value changes, then the <see cref="TextEditorViewModel"/> needs to be re-measured.<br/><br/>The value is mutated in the Blazor 'OnAfterRender(...)' lifecycle method.</summary>
    public RenderStateKey OptionsRenderStateKey { get; set; } = RenderStateKey.Empty;
    public string CommandBarValue { get; set; } = string.Empty;
    public bool ShouldSetFocusAfterNextRender { get; set; }

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

        var textEditor = TextEditorService.ViewModel.FindBackingModelOrDefault(
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
        await TextEditorService.ViewModel.MutateScrollHorizontalPositionAsync(
            BodyElementId,
            GutterElementId,
            pixels);
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        await TextEditorService.ViewModel.MutateScrollVerticalPositionAsync(
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
    
    /// <summary>If a parameter is null the JavaScript will not modify that value</summary>
    public async Task SetScrollPositionAsync(double? scrollLeft, double? scrollTop)
    {
        await TextEditorService.ViewModel.SetScrollPositionAsync(
            BodyElementId,
            GutterElementId,
            scrollLeft,
            scrollTop);
    }

    public async Task FocusAsync()
    {
        await TextEditorService.ViewModel.FocusPrimaryCursorAsync(
            PrimaryCursorContentId);
    }

    public async Task RemeasureAsync(
        TextEditorOptions options,
        string measureCharacterWidthAndRowHeightElementId,
        int countOfTestCharacters)
    {
        var throttledRemeasureEvent =
            await _generalOperationThrottle.FireAsync(
                0,
                CancellationToken.None);

        if (throttledRemeasureEvent.isCancellationRequested)
            return;

        try
        {
            await GeneralOperationSemaphoreSlim.WaitAsync();

            var characterWidthAndRowHeight = await TextEditorService.ViewModel.MeasureCharacterWidthAndRowHeightAsync(
                measureCharacterWidthAndRowHeightElementId,
                countOfTestCharacters);

            VirtualizationResult.CharacterWidthAndRowHeight = characterWidthAndRowHeight;

            TextEditorService.ViewModel.With(
                    ViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        OptionsRenderStateKey = options.RenderStateKey,
                        ModelRenderStateKey = RenderStateKey.Empty,
                        VirtualizationResult = previousViewModel.VirtualizationResult with
                        {
                            CharacterWidthAndRowHeight = characterWidthAndRowHeight
                        },
                        RenderStateKey = RenderStateKey.NewRenderStateKey()
                    });
        }
        finally
        {
            GeneralOperationSemaphoreSlim.Release();
        }
    }

    public async Task CalculateVirtualizationResultAsync(
        TextEditorModel? model,
        ElementMeasurementsInPixels? bodyMeasurementsInPixels,
        bool useThrottling,
        CancellationToken cancellationToken)
    {
        if (useThrottling)
        {
            var throttledCalculateVirtualizationResultEvent =
                await _generalOperationThrottle.FireAsync(
                    0,
                    CancellationToken.None);

            if (throttledCalculateVirtualizationResultEvent.isCancellationRequested)
                return;
        }

        try
        {
            await GeneralOperationSemaphoreSlim.WaitAsync();

            if (cancellationToken.IsCancellationRequested)
                return;

            var localCharacterWidthAndRowHeight = VirtualizationResult.CharacterWidthAndRowHeight;

            if (bodyMeasurementsInPixels is null)
            {
                bodyMeasurementsInPixels = await TextEditorService.ViewModel
                    .MeasureElementInPixelsAsync(BodyElementId);
            }

            _mostRecentBodyMeasurementsInPixels = bodyMeasurementsInPixels;

            bodyMeasurementsInPixels = bodyMeasurementsInPixels with
            {
                MeasurementsExpiredCancellationToken = cancellationToken
            };

            if (model is null ||
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
                    model.RowEndingPositions.Length)
                {
                    verticalTake = model.RowEndingPositions.Length -
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

            var virtualizedEntries = model
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

                        var tabsOnSameRowBeforeCursor = model
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
                model.MostCharactersOnASingleRowTuple.rowLength *
                localCharacterWidthAndRowHeight.CharacterWidthInPixels;

            var totalHeight =
                model.RowEndingPositions.Length *
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

            TextEditorService.ViewModel.With(
                    ViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        ModelRenderStateKey = model.RenderStateKey,
                        VirtualizationResult = virtualizationResult,
                        RenderStateKey = RenderStateKey.NewRenderStateKey()
                    });
        }
        finally
        {
            GeneralOperationSemaphoreSlim.Release();
        }
    }

    public bool IsDirty(TextEditorOptions? options)
    {
        if (options is null)
            return true;

        return OptionsRenderStateKey != options.RenderStateKey;
    }

    public bool IsDirty(TextEditorModel? model)
    {
        if (model is null)
            return true;

        return ModelRenderStateKey != model.RenderStateKey;
    }
}