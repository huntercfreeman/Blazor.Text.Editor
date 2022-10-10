using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorCursorDisplay : ComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public FontWidthAndElementHeight FontWidthAndElementHeight { get; set; } = null!;

    private ElementReference? _textEditorCursorDisplayElementReference;
    private bool _hasBlinkAnimation = true;
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    
    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string BlinkAnimationCssClass => _hasBlinkAnimation
        ? "bte_blink"
        : string.Empty;

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

        return $"{top} {height}";
    }

    public async Task FocusAsync()
    {
        if (_textEditorCursorDisplayElementReference is not null)
            await _textEditorCursorDisplayElementReference.Value.FocusAsync();
    }

    public void Dispose()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
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
}