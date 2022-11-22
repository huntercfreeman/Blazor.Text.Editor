using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.TextEditorDisplayInternals;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public partial class TextEditorDisplay : TextEditorView
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;

    [Parameter]
    public RenderFragment? OnContextMenuRenderFragment { get; set; }
    [Parameter]
    public RenderFragment? AutoCompleteMenuRenderFragment { get; set; }
    [Parameter]
    public Action<TextEditorBase>? OnSaveRequested { get; set; }
    /// <summary>
    /// If left null, the default <see cref="AfterOnKeyDownAsync"/> will
    /// be used.
    /// <br/><br/>
    /// The default <see cref="AfterOnKeyDownAsync"/> will provide
    /// syntax highlighting, and autocomplete.
    /// <br/><br/>
    /// The syntax highlighting occurs on ';', whitespace, paste, undo, redo
    /// <br/><br/>
    /// The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }.
    /// Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/>
    /// and the one can provide their own implementation when registering the
    /// BlazorTextEditor services using <see cref="TextEditorServiceOptions.AutocompleteServiceFactory"/>
    /// <br/><br/>
    /// (TextEditorBase textEditor, ImmutableArray&lt;TextEditorCursorSnapshot&gt; textEditorCursorSnapshots,
    /// KeyboardEventArgs keyboardEventArgs, Func&lt;TextEditorMenuKind, Task&gt; setTextEditorMenuKind), Task
    /// </summary>
    [Parameter]
    public Func<TextEditorBase, ImmutableArray<TextEditorCursorSnapshot>, KeyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task>, Task>? AfterOnKeyDownAsync { get; set; }
    [Parameter]
    public bool ShouldRemeasureFlag { get; set; }
    [Parameter]
    public string StyleCssString { get; set; } = null!;
    [Parameter]
    public string ClassCssString { get; set; } = null!;
    /// <summary>
    ///     TabIndex is used for the html attribute: 'tabindex'
    ///     <br /><br />
    ///     tabindex of -1 means one can only set focus to the
    ///     text editor by clicking on it.
    ///     <br /><br />
    ///     tabindex of 0 means one can both use the tab key to set focus to the
    ///     text editor or click on it.
    /// </summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;
    /// <summary>
    /// <see cref="IncludeHeaderHelperComponent"/> results in
    /// <see cref="TextEditorHeader"/> being rendered above the
    /// <see cref="TextEditorDisplay"/>
    /// <br/><br/>
    /// Default value is true
    /// </summary>
    [Parameter]
    public bool IncludeHeaderHelperComponent { get; set; } = true;
    /// <summary>
    /// <see cref="HeaderButtonKinds"/> contains
    /// the enum value that represents a button displayed
    /// in the <see cref="TextEditorHeader"/>.
    /// <br/><br/>
    /// The <see cref="TextEditorHeader"/> is only displayed if
    /// <see cref="IncludeHeaderHelperComponent"/> is set to true.
    /// </summary>
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }
    /// <summary>
    /// <see cref="IncludeFooterHelperComponent"/> results in
    /// <see cref="TextEditorFooter"/> being rendered below the
    /// <see cref="TextEditorDisplay"/>
    /// <br/><br/>
    /// Default value is true
    /// </summary>
    [Parameter]
    public bool IncludeFooterHelperComponent { get; set; } = true;
    /// <summary>
    /// <see cref="FileExtension"/> is displayed as is within the
    /// <see cref="TextEditorFooter"/>.
    /// <br/><br/>
    /// The <see cref="TextEditorFooter"/> is only displayed if
    /// <see cref="IncludeFooterHelperComponent"/> is set to true.
    /// </summary>
    [Parameter]
    public string FileExtension { get; set; } = string.Empty;
    /// <summary>
    /// <see cref="IncludeDefaultContextMenu"/> results in
    /// <see cref="TextEditorContextMenu"/> being rendered
    /// On a context menu event which includes
    /// <br/>
    /// -ShiftKey + F10
    /// <br/>
    /// -ContextMenu button
    /// <br/>
    /// -RightClick of mouse
    /// <br/><br/>
    /// Default value is true
    /// </summary>
    [Parameter]
    public bool IncludeDefaultContextMenu { get; set; } = true;
    /// <summary>
    /// <see cref="IncludeDefaultAutocompleteMenu"/> results in
    /// <see cref="TextEditorAutocompleteMenu"/> being rendered
    /// when the user types and possible autocomplete
    /// options are available
    /// <br/><br/>
    /// Default value is true
    /// </summary>
    [Parameter]
    public bool IncludeDefaultAutocompleteMenu { get; set; } = true;

    private readonly SemaphoreSlim _afterOnKeyDownSyntaxHighlightingSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _afterOnKeyDownSyntaxHighlightingDelay = TimeSpan.FromSeconds(1);
    private int _skippedSyntaxHighlightingEventCount;

    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);

    private int? _previousGlobalFontSizeInPixels;
    private bool? _previousShouldRemeasureFlag;
    private TextEditorOptions? _previousGlobalTextEditorOptions;

    private TextEditorKey? _previousTextEditorKey;
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private ElementReference _textEditorDisplayElementReference;

    private Guid _textEditorGuid = Guid.NewGuid();

    /// <summary>
    ///     Do not select text just because the user has the Left Mouse Button down.
    ///     They might hold down Left Mouse Button from outside the TextEditorDisplay's content div
    ///     then move their mouse over the content div while holding the Left Mouse Button down.
    ///     <br /><br />
    ///     Instead only select text if an @onmousedown event triggered <see cref="_thinksLeftMouseButtonIsDown" />
    ///     to be equal to true and the @onmousemove event followed afterwards.
    /// </summary>
    private bool _thinksLeftMouseButtonIsDown;

    private VirtualizationDisplay<List<RichCharacter>>? _virtualizationDisplay;
    private TextEditorHeader? _textEditorHeader;
    private TextEditorFooter? _textEditorFooter;
    private MeasureCharacterWidthAndRowHeight? _measureCharacterWidthAndRowHeightComponent;
    
    // TODO: Tracking the most recently rendered virtualization result feels hacky and needs to be looked into further. The need for this arose when implementing the method "CursorMovePageBottomAsync()"
    private VirtualizationResult<List<RichCharacter>>? _mostRecentlyRenderedVirtualizationResult;

    public bool ShouldMeasureDimensions { get; set; } = true;
    public CharacterWidthAndRowHeight? CharacterWidthAndRowHeight { get; private set; }
    public RelativeCoordinates? RelativeCoordinatesOnClick { get; private set; }
    public WidthAndHeightOfTextEditor? WidthAndHeightOfTextEditor { get; private set; }

    public TextEditorBase? MutableReferenceToTextEditor => TextEditorStatesSelection.Value;

    private string TextEditorContentId => $"bte_text-editor-content_{_textEditorGuid}";

    private string MeasureCharacterWidthAndRowHeightElementId =>
        $"bte_measure-character-width-and-row-height_{_textEditorGuid}";

    private MarkupString GetAllTextEscaped => (MarkupString)(MutableReferenceToTextEditor?
                                                                 .GetAllText()
                                                                 .Replace("\r\n", "\\r\\n<br/>")
                                                                 .Replace("\r", "\\r<br/>")
                                                                 .Replace("\n", "\\n<br/>")
                                                                 .Replace("\t", "--->")
                                                                 .Replace(" ", "·")
                                                             ?? string.Empty);

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

    public TextEditorCursor PrimaryCursor { get; } = new(true);

    protected override async Task OnParametersSetAsync()
    {
        var primaryCursorSnapshot = new TextEditorCursorSnapshot(PrimaryCursor);

        var currentGlobalFontSizeInPixels = TextEditorService
            .TextEditorStates
            .GlobalTextEditorOptions
            .FontSizeInPixels!
            .Value;

        var dirtyGlobalFontSizeInPixels =
            _previousGlobalFontSizeInPixels is null ||
            _previousGlobalFontSizeInPixels != currentGlobalFontSizeInPixels;

        var dirtyShouldRemeasureFlag = _previousShouldRemeasureFlag is null ||
                                       _previousShouldRemeasureFlag != ShouldRemeasureFlag;

        if (dirtyGlobalFontSizeInPixels || dirtyShouldRemeasureFlag)
        {
            _previousGlobalFontSizeInPixels = currentGlobalFontSizeInPixels;
            _previousShouldRemeasureFlag = ShouldRemeasureFlag;

            ShouldMeasureDimensions = true;
            await InvokeAsync(StateHasChanged);

            _virtualizationDisplay?.InvokeEntriesProviderFunc();
        }

        if (_previousTextEditorKey is null ||
            _previousTextEditorKey != TextEditorKey)
        {
            // Setting IndexCoordinates to (0, 0) twice in this block
            // due to a general feeling of unease
            // that something bad will happen otherwise.
            {
                primaryCursorSnapshot.UserCursor.IndexCoordinates = (0, 0);
                primaryCursorSnapshot
                    .UserCursor.TextEditorSelection.AnchorPositionIndex = null;

                _previousTextEditorKey = TextEditorKey;

                primaryCursorSnapshot.UserCursor.IndexCoordinates = (0, 0);
                primaryCursorSnapshot
                    .UserCursor.TextEditorSelection.AnchorPositionIndex = null;
            }

            _virtualizationDisplay?.InvokeEntriesProviderFunc();
        }

        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        // base will select the
        // TreeViewBase from the TreeViewKey
        base.OnInitialized();

        TextEditorStatesSelection.SelectedValueChanged += TextEditorStatesSelectionOnSelectedValueChanged;
    }

    private void TextEditorStatesSelectionOnSelectedValueChanged(object? sender, TextEditorBase? e)
    {
        _virtualizationDisplay?.InvokeEntriesProviderFunc();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _virtualizationDisplay?.InvokeEntriesProviderFunc();
        }

        if (ShouldMeasureDimensions)
        {
            CharacterWidthAndRowHeight = await JsRuntime
                .InvokeAsync<CharacterWidthAndRowHeight>(
                    "blazorTextEditor.measureCharacterWidthAndRowHeight",
                    MeasureCharacterWidthAndRowHeightElementId,
                    _measureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0);

            WidthAndHeightOfTextEditor = await JsRuntime
                .InvokeAsync<WidthAndHeightOfTextEditor>(
                    "blazorTextEditor.measureWidthAndHeightOfTextEditor",
                    TextEditorContentId);

            {
                ShouldMeasureDimensions = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task FocusTextEditorAsync()
    {
        if (_textEditorCursorDisplay is not null)
            await _textEditorCursorDisplay.FocusAsync();
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        var safeTextEditorReference = MutableReferenceToTextEditor;

        if (safeTextEditorReference is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(PrimaryCursor);

        var cursorSnapshots = new TextEditorCursorSnapshot[]
        {
            new(primaryCursorSnapshot.UserCursor),
        }.ToImmutableArray();
        
        var command = safeTextEditorReference.TextEditorKeymap.KeymapFunc
            .Invoke(keyboardEventArgs);

        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key) && 
            command is null)
        {
            if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keyboardEventArgs.Key ||
                 KeyboardKeyFacts.MovementKeys.ARROW_UP == keyboardEventArgs.Key) &&
                _textEditorCursorDisplay is not null &&
                _textEditorCursorDisplay.TextEditorMenuKind ==
                TextEditorMenuKind.AutoCompleteMenu)
            {
                _textEditorCursorDisplay.SetFocusToActiveMenu();
            }
            else
            {
                TextEditorCursor.MoveCursor(
                    keyboardEventArgs,
                    primaryCursorSnapshot.UserCursor,
                    safeTextEditorReference);

                _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
            }
        }
        else if (KeyboardKeyFacts.CheckIsContextMenuEvent(keyboardEventArgs))
        {
            _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
        }
        else
        {
            if (command is not null)
            {
                await command.DoAsyncFunc.Invoke(
                    new TextEditorCommandParameter(
                        safeTextEditorReference,
                        cursorSnapshots,
                        ClipboardProvider,
                        TextEditorService,
                        this));
            }
            else
            {
                if (!IsAutocompleteMenuInvoker(keyboardEventArgs))
                {
                    if (!KeyboardKeyFacts.IsMetaKey(keyboardEventArgs)
                        || (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key ||
                            KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key ||
                            KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key))
                    {
                        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
                    }
                }

                Dispatcher.Dispatch(
                    new KeyboardEventTextEditorBaseAction(
                        TextEditorKey,
                        cursorSnapshots,
                        keyboardEventArgs,
                        CancellationToken.None));
            }
        }

        if (keyboardEventArgs.Key != "Shift" &&
            keyboardEventArgs.Key != "Control" &&
            keyboardEventArgs.Key != "Alt" &&
            (command?.ShouldScrollCursorIntoView ?? true))
        {
            primaryCursorSnapshot.UserCursor.ShouldRevealCursor = true;
        }

        var afterOnKeyDownAsync = AfterOnKeyDownAsync
                                  ?? HandleAfterOnKeyDownAsync;

        var cursorDisplay = _textEditorCursorDisplay;

        if (cursorDisplay is not null)
        {
            var textEditor = safeTextEditorReference;

            // Do not block UI thread with long running AfterOnKeyDownAsync 
            _ = Task.Run(async () =>
            {
                await afterOnKeyDownAsync.Invoke(
                    textEditor,
                    cursorSnapshots,
                    keyboardEventArgs,
                    cursorDisplay.SetShouldDisplayMenuAsync);
            });
        }
    }

    private void HandleOnContextMenuAsync()
    {
        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
    }

    private async Task HandleContentOnDoubleClickAsync(MouseEventArgs mouseEventArgs)
    {
        var safeTextEditorReference = MutableReferenceToTextEditor;

        if (safeTextEditorReference is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(PrimaryCursor);

        if ((mouseEventArgs.Buttons & 1) != 1 &&
            TextEditorSelectionHelper.HasSelectedText(
                primaryCursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection))
            // Not pressing the left mouse button
            // so assume ContextMenu is desired result.
            return;

        if (mouseEventArgs.ShiftKey)
            // Do not expand selection if user is holding shift
            return;

        var rowAndColumnIndex =
            await DetermineRowAndColumnIndex(mouseEventArgs);

        var lowerColumnIndexExpansion = safeTextEditorReference
            .GetColumnIndexOfCharacterWithDifferingKind(
                rowAndColumnIndex.rowIndex,
                rowAndColumnIndex.columnIndex,
                true);

        lowerColumnIndexExpansion =
            lowerColumnIndexExpansion == -1
                ? 0
                : lowerColumnIndexExpansion;

        var higherColumnIndexExpansion = safeTextEditorReference
            .GetColumnIndexOfCharacterWithDifferingKind(
                rowAndColumnIndex.rowIndex,
                rowAndColumnIndex.columnIndex,
                false);

        higherColumnIndexExpansion =
            higherColumnIndexExpansion == -1
                ? safeTextEditorReference.GetLengthOfRow(
                    rowAndColumnIndex.rowIndex)
                : higherColumnIndexExpansion;

        // Move user's cursor position to the higher expansion
        {
            primaryCursorSnapshot.UserCursor.IndexCoordinates =
                (rowAndColumnIndex.rowIndex, higherColumnIndexExpansion);

            primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
                rowAndColumnIndex.columnIndex;
        }

        // Set text selection ending to higher expansion
        {
            var cursorPositionOfHigherExpansion = safeTextEditorReference
                .GetPositionIndex(
                    rowAndColumnIndex.rowIndex,
                    higherColumnIndexExpansion);

            primaryCursorSnapshot
                    .UserCursor.TextEditorSelection.EndingPositionIndex =
                cursorPositionOfHigherExpansion;
        }

        // Set text selection anchor to lower expansion
        {
            var cursorPositionOfLowerExpansion = safeTextEditorReference
                .GetPositionIndex(
                    rowAndColumnIndex.rowIndex,
                    lowerColumnIndexExpansion);

            primaryCursorSnapshot
                    .UserCursor.TextEditorSelection.AnchorPositionIndex =
                cursorPositionOfLowerExpansion;
        }
    }

    private async Task HandleContentOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        var safeTextEditorReference = MutableReferenceToTextEditor;

        if (safeTextEditorReference is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(PrimaryCursor);

        if ((mouseEventArgs.Buttons & 1) != 1 &&
            TextEditorSelectionHelper.HasSelectedText(
                primaryCursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection))
            // Not pressing the left mouse button
            // so assume ContextMenu is desired result.
            return;

        _textEditorCursorDisplay?.SetShouldDisplayMenuAsync(
            TextEditorMenuKind.None,
            false);

        var rowAndColumnIndex =
            await DetermineRowAndColumnIndex(mouseEventArgs);

        primaryCursorSnapshot.UserCursor.IndexCoordinates =
            (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
            rowAndColumnIndex.columnIndex;

        _textEditorCursorDisplay?.PauseBlinkAnimation();

        var cursorPositionIndex = safeTextEditorReference
            .GetCursorPositionIndex(
                new TextEditorCursor(rowAndColumnIndex, false));

        if (mouseEventArgs.ShiftKey)
        {
            if (!TextEditorSelectionHelper.HasSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection))
            {
                // If user does not yet have a selection
                // then place the text selection anchor were they were

                var cursorPositionPriorToMovementOccurring = safeTextEditorReference
                    .GetPositionIndex(
                        primaryCursorSnapshot.ImmutableCursor.RowIndex,
                        primaryCursorSnapshot.ImmutableCursor.ColumnIndex);

                primaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                    cursorPositionPriorToMovementOccurring;
            }

            // If user ALREADY has a selection
            // then do not modify the text selection anchor
        }
        else
        {
            primaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                cursorPositionIndex;
        }

        primaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex =
            cursorPositionIndex;

        _thinksLeftMouseButtonIsDown = true;
    }

    /// <summary>
    ///     OnMouseUp is unnecessary
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    private async Task HandleContentOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        var success = await _onMouseMoveSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);

        if (!success)
            return;

        try
        {
            var safeTextEditorReference = MutableReferenceToTextEditor;

            if (safeTextEditorReference is null)
                return;

            var primaryCursorSnapshot = new TextEditorCursorSnapshot(PrimaryCursor);

            // Buttons is a bit flag
            // '& 1' gets if left mouse button is held
            if (_thinksLeftMouseButtonIsDown &&
                (mouseEventArgs.Buttons & 1) == 1)
            {
                var rowAndColumnIndex =
                    await DetermineRowAndColumnIndex(mouseEventArgs);

                primaryCursorSnapshot.UserCursor.IndexCoordinates =
                    (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
                primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
                    rowAndColumnIndex.columnIndex;

                _textEditorCursorDisplay?.PauseBlinkAnimation();

                primaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex =
                    safeTextEditorReference
                        .GetCursorPositionIndex(
                            new TextEditorCursor(rowAndColumnIndex, false));
            }
            else
                _thinksLeftMouseButtonIsDown = false;

            await Task.Delay(_onMouseMoveDelay);
        }
        finally
        {
            _onMouseMoveSemaphoreSlim.Release();
        }
    }

    private async Task<(int rowIndex, int columnIndex)> DetermineRowAndColumnIndex(
        MouseEventArgs mouseEventArgs)
    {
        var safeTextEditorReference = MutableReferenceToTextEditor;

        if (safeTextEditorReference is null)
            return (0, 0);

        RelativeCoordinatesOnClick = await JsRuntime
            .InvokeAsync<RelativeCoordinates>(
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
            positionX -= TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                         TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;
        }

        var columnIndexDouble = positionX / CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var columnIndexInt = (int)Math.Round(
            columnIndexDouble,
            MidpointRounding.AwayFromZero);

        var rowIndex = (int)(positionY / CharacterWidthAndRowHeight.RowHeightInPixels);

        rowIndex = rowIndex > safeTextEditorReference.RowCount - 1
            ? safeTextEditorReference.RowCount - 1
            : rowIndex;

        var lengthOfRow = safeTextEditorReference.GetLengthOfRow(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor =
                columnIndexInt > lengthOfRow
                    ? lengthOfRow
                    : columnIndexInt;

            var tabsOnSameRowBeforeCursor = safeTextEditorReference
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    parameterForGetTabsCountOnSameRowBeforeCursor);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorBase.TAB_WIDTH - 1;

            columnIndexInt -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
        }

        // Line number column offset
        {
            var mostDigitsInARowLineNumber = safeTextEditorReference.RowCount
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

    private VirtualizationResult<List<RichCharacter>>? EntriesProvider(
        VirtualizationRequest request)
    {
        if (CharacterWidthAndRowHeight is null ||
            WidthAndHeightOfTextEditor is null ||
            request.CancellationToken.IsCancellationRequested)
            return null;

        var safeTextEditorReference = TextEditorStatesSelection.Value;

        if (safeTextEditorReference is null)
        {
            return new(
                ImmutableArray<VirtualizationEntry<List<RichCharacter>>>.Empty,
                new(0, 0, 0, 0),
                new(0, 0, 0, 0),
                new(0, 0, 0, 0),
                new(0, 0, 0, 0),
                request.ScrollPosition);
        }

        var verticalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollTopInPixels /
            CharacterWidthAndRowHeight.RowHeightInPixels);

        var verticalTake = (int)Math.Ceiling(
            WidthAndHeightOfTextEditor.HeightInPixels /
            CharacterWidthAndRowHeight.RowHeightInPixels);

        if (verticalStartingIndex + verticalTake >
            safeTextEditorReference.RowEndingPositions.Length)
        {
            verticalTake = safeTextEditorReference.RowEndingPositions.Length -
                           verticalStartingIndex;
        }

        verticalTake = Math.Max(0, verticalTake);

        var horizontalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollLeftInPixels /
            CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var horizontalTake = (int)Math.Ceiling(
            WidthAndHeightOfTextEditor.WidthInPixels /
            CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var virtualizedEntries = safeTextEditorReference
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

                var widthInPixels =
                    horizontallyVirtualizedRow.Count *
                    CharacterWidthAndRowHeight.CharacterWidthInPixels;

                var leftInPixels =
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
            safeTextEditorReference.MostCharactersOnASingleRow *
            CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var totalHeight =
            safeTextEditorReference.RowEndingPositions.Length *
            CharacterWidthAndRowHeight.RowHeightInPixels;

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
            request.ScrollPosition);
    }

    /// <summary>
    /// Default implementation so Syntax Highlighting
    /// can be done with less setup.
    /// <br/><br/>
    /// One can further customize this method however
    /// by passing in to the Func their own version.
    /// </summary>
    public async Task HandleAfterOnKeyDownAsync(
        TextEditorBase textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task> setTextEditorMenuKind)
    {
        var primaryCursorSnapshot = cursorSnapshots
            .First(x =>
                x.UserCursor.IsPrimaryCursor);

        // Indexing can be invoked and this method still check for syntax highlighting and such
        if (IsAutocompleteIndexerInvoker(keyboardEventArgs))
        {
            if (primaryCursorSnapshot.ImmutableCursor.ColumnIndex > 0)
            {
                // All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
                // are to be 1 character long, as well either specific whitespace or punctuation.
                //
                // Therefore 1 character behind might be a word that can be indexed.

                var word = textEditor.ReadPreviousWordOrDefault(
                    primaryCursorSnapshot.ImmutableCursor.RowIndex,
                    primaryCursorSnapshot.ImmutableCursor.ColumnIndex);

                if (word is not null)
                    await AutocompleteIndexer.IndexWordAsync(word);
            }
        }

        if (IsAutocompleteMenuInvoker(keyboardEventArgs))
        {
            await setTextEditorMenuKind.Invoke(
                TextEditorMenuKind.AutoCompleteMenu,
                true);
        }
        else if (IsSyntaxHighlightingInvoker(keyboardEventArgs))
        {
            var success = await _afterOnKeyDownSyntaxHighlightingSemaphoreSlim
                .WaitAsync(TimeSpan.Zero);

            if (!success)
            {
                _skippedSyntaxHighlightingEventCount++;
                return;
            }

            try
            {
                do
                {
                    await textEditor.ApplySyntaxHighlightingAsync();

                    await InvokeAsync(StateHasChanged);

                    await Task.Delay(_afterOnKeyDownSyntaxHighlightingDelay);
                } while (StartSyntaxHighlightEventIfHasSkipped());
            }
            finally
            {
                _afterOnKeyDownSyntaxHighlightingSemaphoreSlim.Release();
            }
        }

        bool StartSyntaxHighlightEventIfHasSkipped()
        {
            if (_skippedSyntaxHighlightingEventCount > 0)
            {
                _skippedSyntaxHighlightingEventCount = 0;

                return true;
            }

            return false;
        }
    }

    private bool IsSyntaxHighlightingInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return keyboardEventArgs.Key == ";" ||
               KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
               (keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "v") ||
               (keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "z") ||
               (keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "y");
    }

    private bool IsAutocompleteMenuInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        // Is {Ctrl + Space} or LetterOrDigit was hit without Ctrl being held
        return (keyboardEventArgs.CtrlKey && keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE) ||
               (!keyboardEventArgs.CtrlKey &&
                !KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) &&
                !KeyboardKeyFacts.IsMetaKey(keyboardEventArgs));
    }

    /// <summary>
    /// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
    /// are to be 1 character long, as well either whitespace or punctuation.
    ///
    /// Therefore 1 character behind might be a word that can be indexed.
    /// </summary>
    private bool IsAutocompleteIndexerInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return (KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
                KeyboardKeyFacts.IsPunctuationCharacter(keyboardEventArgs.Key.First())) &&
               !keyboardEventArgs.CtrlKey;
    }
    
    public async Task MutateScrollHorizontalPositionByPixelsAsync(double pixels)
    {
        await JsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollHorizontalPositionByPixels",
            TextEditorContentId,
            pixels);
        
        await InvokeAsync(StateHasChanged);
        _virtualizationDisplay?.InvokeEntriesProviderFunc();
    }
    
    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        await JsRuntime.InvokeVoidAsync(
            "blazorTextEditor.mutateScrollVerticalPositionByPixels",
            TextEditorContentId,
            pixels);
        
        await InvokeAsync(StateHasChanged);
        _virtualizationDisplay?.InvokeEntriesProviderFunc();
    }

    public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            lines * (CharacterWidthAndRowHeight?.RowHeightInPixels ?? 0));
    }
    
    public async Task MutateScrollVerticalPositionByPagesAsync(double pages)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            pages * (WidthAndHeightOfTextEditor?.HeightInPixels ?? 0));
    }
    
    public async Task CursorMovePageBottomAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = _mostRecentlyRenderedVirtualizationResult;
        var textEditor = TextEditorStatesSelection.Value;

        if ((localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false) &&
            textEditor is not null)
        {
            var lastEntry = localMostRecentlyRenderedVirtualizationResult.Entries.Last();

            var lastEntriesRowLength = textEditor.GetLengthOfRow(lastEntry.Index);
            
            PrimaryCursor.IndexCoordinates = (lastEntry.Index, lastEntriesRowLength);
        }
    }
    
    public async Task CursorMovePageTopAsync()
    {
        var localMostRecentlyRenderedVirtualizationResult = _mostRecentlyRenderedVirtualizationResult;
        var textEditor = TextEditorStatesSelection.Value;

        if ((localMostRecentlyRenderedVirtualizationResult?.Entries.Any() ?? false) &&
            textEditor is not null)
        {
            var firstEntry = localMostRecentlyRenderedVirtualizationResult.Entries.First();

            PrimaryCursor.IndexCoordinates = (firstEntry.Index, 0);
        }
    }
    
    private async Task HandleOnWheelAsync(WheelEventArgs wheelEventArgs)
    {
        if (wheelEventArgs.ShiftKey)
        {
            await MutateScrollHorizontalPositionByPixelsAsync(
                wheelEventArgs.DeltaY);
        }
        else
        {
            await MutateScrollVerticalPositionByPixelsAsync(
                wheelEventArgs.DeltaY);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TextEditorStatesSelection.SelectedValueChanged -= TextEditorStatesSelectionOnSelectedValueChanged;
        }

        base.Dispose(true);
    }
}