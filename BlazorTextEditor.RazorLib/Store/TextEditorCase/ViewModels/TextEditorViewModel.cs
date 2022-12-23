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

    public string TextEditorContentId => $"bte_text-editor-content_{TextEditorViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"bte_text-editor-content_{TextEditorViewModelKey.Guid}_primary-cursor";

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
            TextEditorContentId,
            pixels);
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        await TextEditorService.MutateScrollVerticalPositionByPixelsAsync(
            TextEditorContentId,
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
            TextEditorContentId,
            scrollLeft,
            scrollTop);
    }

    public async Task FocusTextEditorAsync()
    {
        await TextEditorService.FocusPrimaryCursorAsync(
            PrimaryCursorContentId);
    }
}