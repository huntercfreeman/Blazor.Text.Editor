using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Misc;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;

public record TextEditorViewModel(
    TextEditorViewModelKey TextEditorViewModelKey,
    TextEditorKey TextEditorKey,
    Func<TextEditorBase> GetTextEditorBaseFunc,
    IJSRuntime JsRuntime)
{
    // TODO: Tracking the most recently rendered virtualization result feels hacky and needs to be looked into further. The need for this arose when implementing the method "CursorMovePageBottomAsync()"
    public VirtualizationResult<List<RichCharacter>>? MostRecentlyRenderedVirtualizationResult { get; set; }
    
    public TextEditorCursor PrimaryCursor { get; } = new(true);
    public TextEditorRenderStateKey TextEditorRenderStateKey { get; init; } = TextEditorRenderStateKey.NewTextEditorRenderStateKey();

    public string TextEditorContentId => $"bte_text-editor-content_{TextEditorViewModelKey.Guid}";

    public bool ShouldMeasureDimensions { get; set; } = true;
    public Action<TextEditorBase>? OnSaveRequested { get; set; }
    
    public CharacterWidthAndRowHeight? CharacterWidthAndRowHeight { get; set; }
    public WidthAndHeightOfTextEditor? WidthAndHeightOfTextEditor { get; set; }

    public async Task CursorMovePageTopAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = MostRecentlyRenderedVirtualizationResult;
        var textEditor = GetTextEditorBaseFunc.Invoke();

        if (localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false)
        {
            var firstEntry = localMostRecentlyRenderedVirtualizationResult.Entries.First();

            PrimaryCursor.IndexCoordinates = (firstEntry.Index, 0);
        }
    }

    public async Task CursorMovePageBottomAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = MostRecentlyRenderedVirtualizationResult;
        var textEditor = GetTextEditorBaseFunc.Invoke();

        if (localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false)
        {
            var lastEntry = localMostRecentlyRenderedVirtualizationResult.Entries.Last();

            var lastEntriesRowLength = textEditor.GetLengthOfRow(lastEntry.Index);
            
            PrimaryCursor.IndexCoordinates = (lastEntry.Index, lastEntriesRowLength);
        }
    }
    
    public async Task MutateScrollHorizontalPositionByPixelsAsync(double pixels)
    {
        await JsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollHorizontalPositionByPixels",
            TextEditorContentId,
            pixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();

        // TODO: await ForceVirtualizationInvocation();
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        await JsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollVerticalPositionByPixels",
            TextEditorContentId,
            pixels);
        
        // Blazor WebAssembly as of this comment is single threaded and
        // the UI freezes without this await Task.Yield
        await Task.Yield();
        
        // TODO: await ForceVirtualizationInvocation();
    }

    public async Task MutateScrollVerticalPositionByPagesAsync(double pages)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            pages * (WidthAndHeightOfTextEditor?.HeightInPixels ?? 0));
    }

    public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            lines * (CharacterWidthAndRowHeight?.RowHeightInPixels ?? 0));
    }

    public async Task FocusTextEditorAsync()
    {
        // TODO: FocusTextEditorAsync
    }
}