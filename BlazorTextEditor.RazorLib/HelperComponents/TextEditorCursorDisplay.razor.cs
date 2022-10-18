using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorCursorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public FontWidthAndElementHeight FontWidthAndElementHeight { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ScrollableContainerId { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsFocusTarget { get; set; }
    [Parameter]
    public RenderFragment? OnContextMenuRenderFragment { get; set; }

    private ElementReference? _textEditorCursorDisplayElementReference;
    private bool _shouldDisplayContextMenu;
    private bool _hasBlinkAnimation = true;
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    
    public string TextEditorCursorDisplayId => $"bte_text-editor-cursor-display_{_intersectionObserverMapKey}";
    
    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string ContextMenuStyleCss => GetContextMenuStyleCss();
    public string BlinkAnimationCssClass => _hasBlinkAnimation
        ? "bte_blink"
        : string.Empty;

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
        var leftInPixels = 0d;
        
        // Gutter padding column offset
        {
            leftInPixels += 
                (TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels);
        }
        
        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = TextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex, 
                    TextEditorCursor.IndexCoordinates.columnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            leftInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * FontWidthAndElementHeight.FontWidthInPixels);
        }
        
        // Line number column offset
        {
            var mostDigitsInARowLineNumber = TextEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * FontWidthAndElementHeight.FontWidthInPixels;
        }

        leftInPixels += FontWidthAndElementHeight.FontWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;
        
        var left = $"left: {leftInPixels}px;";
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {FontWidthAndElementHeight.ElementHeightInPixels}px;";

        return $"{left} {top} {height}";
    }

    private string GetCaretRowStyleCss()
    {
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {FontWidthAndElementHeight.ElementHeightInPixels}px;";

        var width = $"width: {TextEditor.MostCharactersOnASingleRow * FontWidthAndElementHeight.FontWidthInPixels}px;";
        
        return $"{top} {width} {height}";
    }
    
    private string GetContextMenuStyleCss()
    {
        var leftInPixels = 0d;
        
        // Gutter padding column offset
        {
            leftInPixels += 
                (TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels);
        }
        
        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = TextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex, 
                    TextEditorCursor.IndexCoordinates.columnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            leftInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * FontWidthAndElementHeight.FontWidthInPixels);
        }
        
        // Line number column offset
        {
            var mostDigitsInARowLineNumber = TextEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * FontWidthAndElementHeight.FontWidthInPixels;
        }

        leftInPixels += FontWidthAndElementHeight.FontWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;
        
        var left = $"left: {leftInPixels}px;";
        
        // Top is 1 row further than the cursor so it does not cover text at cursor position.
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * (TextEditorCursor.IndexCoordinates.rowIndex + 1)}px;";
        
        var minWidth = $"min-Width: {FontWidthAndElementHeight.FontWidthInPixels * 16}px;";
        var minHeight = $"min-height: {FontWidthAndElementHeight.ElementHeightInPixels * 4}px;";

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
        _blinkingCursorCancellationTokenSource = new();

        return _blinkingCursorCancellationTokenSource.Token;
    }
    
    public async Task SetShouldDisplayContextMenuAsync(bool value)
    {
        _shouldDisplayContextMenu = value;
        await InvokeAsync(StateHasChanged);

        if (!_shouldDisplayContextMenu)
        {
            await FocusAsync();
        }
    }
    
    public void Dispose()
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
}