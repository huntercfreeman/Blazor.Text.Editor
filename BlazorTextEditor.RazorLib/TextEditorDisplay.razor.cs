using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public partial class TextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;
    [Parameter]
    public RenderFragment? OnContextMenuRenderFragment { get; set; }
    [Parameter]
    public RenderFragment? AutoCompleteMenuRenderFragment { get; set; }
    [Parameter]
    public Action<TextEditorBase>? OnSaveRequested { get; set; }
    /// <summary>
    /// (TextEditorBase textEditor, ImmutableTextEditorCursor immutablePrimaryCursor, KeyboardEventArgs keyboardEventArgs, Func&lt;TextEditorMenuKind, Task&gt; setTextEditorMenuKind), Task
    /// </summary>
    [Parameter]
    public Func<TextEditorBase, ImmutableTextEditorCursor, KeyboardEventArgs, Func<TextEditorMenuKind, Task> , Task>? AfterOnKeyDownAsync { get; set; }
    [Parameter]
    public bool ShouldRemeasureFlag { get; set; }
    [Parameter]
    public string StyleCssString { get; set; } = null!;
    [Parameter]
    public string ClassCssString { get; set; } = null!;
    /// <summary>
    /// TabIndex is used for the html attribute: 'tabindex'
    /// <br/><br/>
    /// tabindex of -1 means one can only set focus to the
    /// text editor by clicking on it.
    /// <br/><br/>
    /// tabindex of 0 means one can both use the tab key to set focus to the
    /// text editor or click on it.
    /// </summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;
    
    private Guid _textEditorGuid = Guid.NewGuid();
    private ElementReference _textEditorDisplayElementReference;
    private string _testStringForMeasurement = "abcdefghijklmnopqrstuvwxyz0123456789";
    private int _testStringRepeatCount = 6;
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private bool _showGetAllTextEscaped;
    /// <summary>
    /// Do not select text just because the user has the Left Mouse Button down.
    /// They might hold down Left Mouse Button from outside the TextEditorDisplay's content div
    /// then move their mouse over the content div while holding the Left Mouse Button down.
    /// <br/><br/>
    /// Instead only select text if an @onmousedown event triggered <see cref="_thinksLeftMouseButtonIsDown"/>
    /// to be equal to true and the @onmousemove event followed afterwards.
    /// </summary>
    private bool _thinksLeftMouseButtonIsDown;

    private TextEditorKey? _previousTextEditorKey;
    private int? _previousGlobalFontSizeInPixels; 
    private bool? _previousShouldRemeasureFlag; 

    private VirtualizationDisplay<List<RichCharacter>>? _virtualizationDisplay;

    public bool ShouldMeasureDimensions { get; set; } = true;
    public FontWidthAndElementHeight? CharacterWidthAndRowHeight { get; private set; }
    public RelativeCoordinates? RelativeCoordinatesOnClick { get; private set; }
    public WidthAndHeightOfElement? TextEditorWidthAndHeight { get; private set; }

    private string TextEditorContentId => $"bte_text-editor-content_{_textEditorGuid}";
    private string MeasureCharacterWidthAndRowHeightId => $"bte_measure-character-width-and-row-height_{_textEditorGuid}";
    private MarkupString GetAllTextEscaped => (MarkupString) TextEditorStatesSelection.Value
        .GetAllText()
        .Replace("\r\n", "\\r\\n<br/>")
        .Replace("\r", "\\r<br/>")
        .Replace("\n", "\\n<br/>")
        .Replace("\t", "--->")
        .Replace(" ", "·");

    private string GlobalThemeCssClassString => TextEditorService
        .TextEditorStates
        .GlobalTextEditorOptions
        .Theme?
        .CssClassString
    ?? string.Empty; 
    
    private string GlobalFontSizeInPixelsStyling => "font-size: " + TextEditorService
        .TextEditorStates
        .GlobalTextEditorOptions
        .FontSizeInPixels!.Value
    + "px;"; 
    
    private bool GlobalShowNewlines => TextEditorService
        .TextEditorStates.GlobalTextEditorOptions.ShowNewlines!.Value;
    
    private bool GlobalShowWhitespace => TextEditorService
        .TextEditorStates.GlobalTextEditorOptions.ShowWhitespace!.Value;

    public TextEditorCursor PrimaryCursor { get; } = new();
    
    public event Action? CursorsChanged;

    protected override async Task OnParametersSetAsync()
    {
        var currentGlobalFontSizeInPixels = TextEditorService
            .TextEditorStates
            .GlobalTextEditorOptions
            .FontSizeInPixels!
            .Value;

        var dirtyGlobalFontSizeInPixels = _previousGlobalFontSizeInPixels is null ||
                                          _previousGlobalFontSizeInPixels != currentGlobalFontSizeInPixels;

        var dirtyShouldRemeasureFlag = _previousShouldRemeasureFlag is null ||
                                       _previousShouldRemeasureFlag != ShouldRemeasureFlag;
        
        if (dirtyGlobalFontSizeInPixels || dirtyShouldRemeasureFlag)
        {
            _previousGlobalFontSizeInPixels = currentGlobalFontSizeInPixels;
            _previousShouldRemeasureFlag = ShouldRemeasureFlag;
            
            ShouldMeasureDimensions = true;
            await InvokeAsync(StateHasChanged);
            
            ReloadVirtualizationDisplay();
        }
        
        if (_previousTextEditorKey is null ||
            _previousTextEditorKey != TextEditorKey)
        {
            // Setting IndexCoordinates to (0, 0) twice in this block
            // due to a general feeling of unease
            // that something bad will happen otherwise.
            {
                PrimaryCursor.IndexCoordinates = (0, 0);
                PrimaryCursor.TextEditorSelection.AnchorPositionIndex = null;
                
                _previousTextEditorKey = TextEditorKey;
                
                PrimaryCursor.IndexCoordinates = (0, 0);    
                PrimaryCursor.TextEditorSelection.AnchorPositionIndex = null;
            }
            
            ReloadVirtualizationDisplay();
        }

        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorList
                .Single(x => x.Key == TextEditorKey));
        
        CursorsChanged?.Invoke();
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ReloadVirtualizationDisplay();
        }

        if (ShouldMeasureDimensions)
        {
            CharacterWidthAndRowHeight = await JsRuntime.InvokeAsync<FontWidthAndElementHeight>(
                "blazorTextEditor.measureFontWidthAndElementHeightByElementId",
                MeasureCharacterWidthAndRowHeightId,
                _testStringRepeatCount * _testStringForMeasurement.Length);
            
            TextEditorWidthAndHeight = await JsRuntime.InvokeAsync<WidthAndHeightOfElement>(
                "blazorTextEditor.measureWidthAndHeightByElementId",
                TextEditorContentId);

            {
                ShouldMeasureDimensions = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    public void ReloadVirtualizationDisplay()
    {
        _virtualizationDisplay?.InvokeEntriesProviderFunc();
    }

    private async Task FocusTextEditorOnClickAsync()
    {
        if (_textEditorCursorDisplay is not null) 
            await _textEditorCursorDisplay.FocusAsync();
    }
    
    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
        
        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key))
        {
            TextEditorCursor.MoveCursor(
                keyboardEventArgs, 
                PrimaryCursor, 
                TextEditorStatesSelection.Value);
        }
        else if (KeyboardKeyFacts.CheckIsContextMenuEvent(keyboardEventArgs))
        {
            _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
        }
        else
        {
            if (keyboardEventArgs.Key == "c" && keyboardEventArgs.CtrlKey)
            {
                var result = PrimaryCursor.GetSelectedText(TextEditorStatesSelection.Value);
                
                if (result is not null)
                    await ClipboardProvider.SetClipboard(result);
            }
            else if (keyboardEventArgs.Key == "v" && keyboardEventArgs.CtrlKey)
            {
                var clipboard = await ClipboardProvider.ReadClipboard();

                var previousCharacterWasCarriageReturn = false;
        
                foreach (var character in clipboard)
                {
                    if (previousCharacterWasCarriageReturn &&
                        character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
                    {
                        previousCharacterWasCarriageReturn = false;
                        continue;
                    }
            
                    var code = character switch
                    {
                        '\r' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                        '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                        '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                        ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                        _ => character.ToString()
                    };
 
                    TextEditorService.EditTextEditor(new EditTextEditorBaseAction(TextEditorKey,
                        new (ImmutableTextEditorCursor, TextEditorCursor)[]
                        {
                            (new ImmutableTextEditorCursor(PrimaryCursor), PrimaryCursor)
                        }.ToImmutableArray(),
                        new KeyboardEventArgs
                        {
                            Code = code,
                            Key = character.ToString()
                        },
                        CancellationToken.None));

                    previousCharacterWasCarriageReturn = KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN
                                                         == character;
                }

                ReloadVirtualizationDisplay();
            }
            else if (keyboardEventArgs.Key == "s" && keyboardEventArgs.CtrlKey)
            {
                OnSaveRequested?.Invoke(TextEditorStatesSelection.Value);
            }
            else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE &&
                     keyboardEventArgs.CtrlKey ||
                     keyboardEventArgs.AltKey &&
                     keyboardEventArgs.Key == "a")
            {
                // Short term hack to avoid autocomplete keybind being typed.
            }
            else
            {
                Dispatcher.Dispatch(new EditTextEditorBaseAction(TextEditorKey,
                    new (ImmutableTextEditorCursor, TextEditorCursor)[]
                    {
                        (new ImmutableTextEditorCursor(PrimaryCursor), PrimaryCursor)
                    }.ToImmutableArray(),
                    keyboardEventArgs,
                    CancellationToken.None));
            }

            ReloadVirtualizationDisplay();
        }
        
        CursorsChanged?.Invoke();
        
        PrimaryCursor.ShouldRevealCursor = true;

        var afterOnKeyDownAsync = AfterOnKeyDownAsync;
        
        if (afterOnKeyDownAsync is not null)
        {
            var cursorDisplay = _textEditorCursorDisplay;
            
            if (cursorDisplay is not null)
            {
                var textEditor = TextEditorStatesSelection.Value;
                var immutableTextCursor = new ImmutableTextEditorCursor(PrimaryCursor);
            
                // Do not block UI thread with long running AfterOnKeyDownAsync 
                _ = Task.Run(async () =>
                {
                    await afterOnKeyDownAsync.Invoke(
                        textEditor,
                        immutableTextCursor,
                        keyboardEventArgs,
                        cursorDisplay.SetShouldDisplayMenuAsync);
                });
            }
        }
    }
    
    private void HandleOnContextMenuAsync()
    {
        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
    }
    
    private async Task HandleContentOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        if ((mouseEventArgs.Buttons & 1) != 1 &&
            PrimaryCursor.TextEditorSelection.HasSelectedText())
        {
            // Not pressing the left mouse button
            // so assume ContextMenu is desired result.

            return;
        }
        
        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
        
        var rowAndColumnIndex = await DetermineRowAndColumnIndex(mouseEventArgs);

        PrimaryCursor.IndexCoordinates = (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        PrimaryCursor.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

        _textEditorCursorDisplay?.PauseBlinkAnimation();

        var cursorPositionIndex = TextEditorStatesSelection.Value.GetCursorPositionIndex(new TextEditorCursor(rowAndColumnIndex));
        
        PrimaryCursor.TextEditorSelection.AnchorPositionIndex = cursorPositionIndex;

        PrimaryCursor.TextEditorSelection.EndingPositionIndex = cursorPositionIndex;
        
        _thinksLeftMouseButtonIsDown = true;
        
        CursorsChanged?.Invoke();
    }
    
    /// <summary>
    /// OnMouseUp is unnecessary
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    private async Task HandleContentOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        // Buttons is a bit flag
        // '& 1' gets if left mouse button is held
        if (_thinksLeftMouseButtonIsDown && 
            (mouseEventArgs.Buttons & 1) == 1)
        {
            var rowAndColumnIndex = await DetermineRowAndColumnIndex(mouseEventArgs);
            
            PrimaryCursor.IndexCoordinates = (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
            PrimaryCursor.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

            _textEditorCursorDisplay?.PauseBlinkAnimation();
            
            PrimaryCursor.TextEditorSelection.EndingPositionIndex =
                TextEditorStatesSelection.Value.GetCursorPositionIndex(new TextEditorCursor(rowAndColumnIndex));
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;
        }
        
        CursorsChanged?.Invoke();
    }
    
    private async Task<(int rowIndex, int columnIndex)> DetermineRowAndColumnIndex(MouseEventArgs mouseEventArgs)
    {
        var localTextEditor = TextEditorStatesSelection.Value;
        
        RelativeCoordinatesOnClick = await JsRuntime.InvokeAsync<RelativeCoordinates>(
            "blazorTextEditor.getRelativePosition",
            TextEditorContentId,
            mouseEventArgs.ClientX,
            mouseEventArgs.ClientY);

        if (CharacterWidthAndRowHeight is null)
            return (0, 0);

        var positionX = RelativeCoordinatesOnClick.RelativeX;
        var positionY = RelativeCoordinatesOnClick.RelativeY;
        
        // Scroll position offset
        {
            positionX += RelativeCoordinatesOnClick.RelativeScrollLeft;
            positionY += RelativeCoordinatesOnClick.RelativeScrollTop;
        }
        
        // Gutter padding column offset
        {
            positionX -= 
                (TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels);
        }
        
        var columnIndexDouble = positionX / CharacterWidthAndRowHeight.FontWidthInPixels;

        var columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
        
        var rowIndex = (int) (positionY / CharacterWidthAndRowHeight.ElementHeightInPixels);

        rowIndex = rowIndex > localTextEditor.RowCount - 1
            ? localTextEditor.RowCount - 1
            : rowIndex;

        var lengthOfRow = localTextEditor.GetLengthOfRow(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor = columnIndexInt > lengthOfRow
                ? lengthOfRow
                : columnIndexInt;

            var tabsOnSameRowBeforeCursor = localTextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    parameterForGetTabsCountOnSameRowBeforeCursor);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            columnIndexInt -= (extraWidthPerTabKey * tabsOnSameRowBeforeCursor);
        }
        
        // Line number column offset
        {
            var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
                .ToString()
                .Length;

            columnIndexInt -= mostDigitsInARowLineNumber;
        }
        
        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);
        
        return (rowIndex, columnIndexInt);
    }

    private string GetCssClass(byte decorationByte)
    {
        var localTextEditor = TextEditorStatesSelection.Value;

        return localTextEditor.DecorationMapper.Map(decorationByte);
    }

    private string GetRowStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        if (CharacterWidthAndRowHeight is null)
            return string.Empty;
        
        var top = $"top: {index * CharacterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {CharacterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;
        
        var widthOfGutterInPixels = mostDigitsInARowLineNumber * CharacterWidthAndRowHeight.FontWidthInPixels;
        var left = $"left: {widthOfGutterInPixels + TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels + virtualizedRowLeftInPixels}px;";
        
        return $"{top} {height} {left}";
    }

    private string GetGutterStyleCss(int index, double? virtualizedRowLeftInPixels)
    {
        if (CharacterWidthAndRowHeight is null)
            return string.Empty;
        
        var top = $"top: {index * CharacterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {CharacterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;

        var widthInPixels = mostDigitsInARowLineNumber * CharacterWidthAndRowHeight.FontWidthInPixels;

        widthInPixels += TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels;
        
        var width = $"width: {widthInPixels}px;";

        var paddingLeft = $"padding-left: {TextEditorBase.GutterPaddingLeftInPixels}px;";
        var paddingRight = $"padding-right: {TextEditorBase.GutterPaddingRightInPixels}px;";
        
        var left = $"left: {virtualizedRowLeftInPixels}px;";
        
        return $"{left} {top} {height} {width} {paddingLeft} {paddingRight}";
    }

    private string GetTextSelectionStyleCss(int lowerBound, int upperBound, int rowIndex)
    {
        if (CharacterWidthAndRowHeight is null ||
            rowIndex >= TextEditorStatesSelection.Value.RowEndingPositions.Length)
        {
            return string.Empty;
        }
        
        var startOfRowTuple = TextEditorStatesSelection.Value.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorStatesSelection.Value.RowEndingPositions[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = endOfRowTuple.positionIndex 
                                         - 1;

        var fullWidthOfRowIsSelected = true;
        
        if (lowerBound > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex = lowerBound
                                           - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (upperBound < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex = upperBound 
                                         - startOfRowTuple.positionIndex;
            
            fullWidthOfRowIsSelected = false;
        }
        
        var top = $"top: {rowIndex * CharacterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {CharacterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;
        
        var widthOfGutterInPixels = mostDigitsInARowLineNumber * CharacterWidthAndRowHeight.FontWidthInPixels;

        var gutterSizeInPixels = widthOfGutterInPixels 
                + TextEditorBase.GutterPaddingLeftInPixels 
                + TextEditorBase.GutterPaddingRightInPixels;
        
        var selectionStartInPixels = selectionStartingColumnIndex 
                                     * CharacterWidthAndRowHeight.FontWidthInPixels;
        
        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorStatesSelection.Value
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    selectionStartingColumnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            selectionStartInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * CharacterWidthAndRowHeight.FontWidthInPixels);    
        }
        
        var left = $"left: {gutterSizeInPixels + selectionStartInPixels}px;";

        var selectionWidthInPixels = selectionEndingColumnIndex 
                                     * CharacterWidthAndRowHeight.FontWidthInPixels
                                     - selectionStartInPixels;
        
        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorStatesSelection.Value
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    selectionEndingColumnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            selectionWidthInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * CharacterWidthAndRowHeight.FontWidthInPixels);    
        }
        
        var widthCssStyleString = "width: ";

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += "100%";
        else if (selectionStartingColumnIndex != 0 && upperBound > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc(100% - {selectionStartInPixels}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixels}px;";
        
        return $"{top} {height} {left} {widthCssStyleString}";
    }

    private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }

    private VirtualizationResult<List<RichCharacter>>? EntriesProvider(VirtualizationRequest request)
    {
        if (CharacterWidthAndRowHeight is null ||
            TextEditorWidthAndHeight is null ||
            request.CancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var localTextEditor = TextEditorStatesSelection.Value;

        var verticalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollTopInPixels
            / CharacterWidthAndRowHeight.ElementHeightInPixels);

        var verticalTake = (int)Math.Ceiling(
            TextEditorWidthAndHeight.HeightInPixels 
            / CharacterWidthAndRowHeight.ElementHeightInPixels);

        if (verticalStartingIndex + verticalTake > localTextEditor.RowEndingPositions.Length)
            verticalTake = localTextEditor.RowEndingPositions.Length - verticalStartingIndex;

        verticalTake = Math.Max(0, verticalTake);

        var horizontalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollLeftInPixels
            / CharacterWidthAndRowHeight.FontWidthInPixels);

        var horizontalTake = (int)Math.Ceiling(
            TextEditorWidthAndHeight.WidthInPixels / 
            CharacterWidthAndRowHeight.FontWidthInPixels);

        var virtualizedEntries = localTextEditor
            .GetRows(verticalStartingIndex, verticalTake)
            .Select((row, index) =>
            {
                index += verticalStartingIndex;
                
                var localHorizontalTake = horizontalTake;

                if (horizontalStartingIndex + localHorizontalTake > row.Count)
                    localHorizontalTake = row.Count - horizontalStartingIndex;
                
                localHorizontalTake = Math.Max(0, localHorizontalTake);

                var horizontallyVirtualizedRow = row
                    .Skip(horizontalStartingIndex)
                    .Take(localHorizontalTake)
                    .ToList();

                return new VirtualizationEntry<List<RichCharacter>>(
                    index,
                    horizontallyVirtualizedRow,
                    horizontallyVirtualizedRow.Count * CharacterWidthAndRowHeight.FontWidthInPixels,
                    CharacterWidthAndRowHeight.ElementHeightInPixels,
                    horizontalStartingIndex * CharacterWidthAndRowHeight.FontWidthInPixels,
                    index * CharacterWidthAndRowHeight.ElementHeightInPixels);
            }).ToImmutableArray();

        var totalWidth = localTextEditor.MostCharactersOnASingleRow 
                         * CharacterWidthAndRowHeight.FontWidthInPixels;

        var totalHeight = localTextEditor.RowEndingPositions.Length * 
                          CharacterWidthAndRowHeight.ElementHeightInPixels;
        
        var leftBoundary = new VirtualizationBoundary(
            WidthInPixels: horizontalStartingIndex * CharacterWidthAndRowHeight.FontWidthInPixels,
            HeightInPixels: null,
            LeftInPixels: 0,
            TopInPixels: 0);

        var rightBoundaryLeftInPixels = leftBoundary.WidthInPixels +
                                        CharacterWidthAndRowHeight.FontWidthInPixels * horizontalTake;
        
        var rightBoundary = new VirtualizationBoundary(
            WidthInPixels: totalWidth - rightBoundaryLeftInPixels,
            HeightInPixels: null,
            LeftInPixels: rightBoundaryLeftInPixels,
            TopInPixels: 0);
        
        var topBoundary = new VirtualizationBoundary(
            WidthInPixels: null,
            HeightInPixels: verticalStartingIndex * CharacterWidthAndRowHeight.ElementHeightInPixels,
            LeftInPixels: 0,
            TopInPixels: 0);
        
        var bottomBoundaryTopInPixels = topBoundary.HeightInPixels +
                                        CharacterWidthAndRowHeight.ElementHeightInPixels * verticalTake;
        
        var bottomBoundary = new VirtualizationBoundary(
            WidthInPixels: null,
            HeightInPixels: totalHeight - bottomBoundaryTopInPixels,
            LeftInPixels: 0,
            TopInPixels: bottomBoundaryTopInPixels);

        return new VirtualizationResult<List<RichCharacter>>(
            virtualizedEntries,
            leftBoundary,
            rightBoundary,
            topBoundary,
            bottomBoundary);
    }
}
