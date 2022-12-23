﻿using System.Collections.Immutable;
using System.Text;
using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.TextEditorDisplayInternals;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib;

public partial class TextEditorViewModelDisplay : TextEditorView
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
    private readonly TimeSpan _afterOnKeyDownSyntaxHighlightingDelay = TimeSpan.FromMilliseconds(750);
    private int _skippedSyntaxHighlightingEventCount;

    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);

    private int? _previousGlobalFontSizeInPixels;
    private bool? _previousShouldRemeasureFlag;
    private TextEditorOptions? _previousGlobalTextEditorOptions;

    private TextEditorKey? _previousTextEditorKey;
    private TextEditorViewModelKey? _previousTextEditorViewModelKey = TextEditorViewModelKey.Empty;
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private ElementReference _textEditorDisplayElementReference;
    
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

    public RelativeCoordinates? RelativeCoordinatesOnClick { get; private set; }

    private Guid _componentHtmlElementId = Guid.NewGuid();
    private WidthAndHeightOfTextEditor? _widthAndHeightOfTextEditorEntirety;

    private string MeasureCharacterWidthAndRowHeightElementId =>
        $"bte_measure-character-width-and-row-height_{_componentHtmlElementId}";
    
    private string ContentElementId =>
        $"bte_text-editor-content_{_componentHtmlElementId}";

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

    protected override async Task OnParametersSetAsync()
    {
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;
        
        var primaryCursorSnapshot = new TextEditorCursorSnapshot(
            safeTextEditorViewModel?.PrimaryCursor ?? new TextEditorCursor(true));

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

        if (safeTextEditorViewModel is not null &&
            (dirtyGlobalFontSizeInPixels || dirtyShouldRemeasureFlag))
        {
            _previousGlobalFontSizeInPixels = currentGlobalFontSizeInPixels;
            _previousShouldRemeasureFlag = ShouldRemeasureFlag;

            safeTextEditorViewModel.ShouldMeasureDimensions = true;
            await InvokeAsync(StateHasChanged);

            await ForceVirtualizationInvocation();
        }

        if (_previousTextEditorViewModelKey != TextEditorViewModelKey)
        {
            _previousTextEditorViewModelKey = TextEditorViewModelKey;
            
            primaryCursorSnapshot.UserCursor.ShouldRevealCursor = true;
            
            await ForceVirtualizationInvocation();
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var textEditorViewModel = ReplaceableTextEditorViewModel;
        
        if (firstRender && 
            textEditorViewModel is not null)
        {
            await ForceVirtualizationInvocation();

            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.preventDefaultOnWheelEvents",
                ContentElementId);
        }

        if (textEditorViewModel is not null && 
            textEditorViewModel.ShouldMeasureDimensions)
        {
            // Capture 'var textEditorModel' early to get a snapshot at
            // this instant of time what the state is as it might change.
            var textEditorModel = TextEditorStatesSelection.Value;
            
            textEditorViewModel.CharacterWidthAndRowHeight = await JsRuntime
                .InvokeAsync<CharacterWidthAndRowHeight>(
                    "blazorTextEditor.measureCharacterWidthAndRowHeight",
                    MeasureCharacterWidthAndRowHeightElementId,
                    _measureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0);

            // TODO: Change the name of 'WidthAndHeightOfTextEditor' class as it is confusingly being used for the TextEditor in its entirety and the body (body meaning entirety - gutter)
            _widthAndHeightOfTextEditorEntirety = await JsRuntime
                .InvokeAsync<WidthAndHeightOfTextEditor>(
                    "blazorTextEditor.measureWidthAndHeightOfTextEditor",
                    ContentElementId);

            var mostDigitsInARowLineNumber = (textEditorModel?.RowCount ?? 0)
                .ToString()
                .Length;

            var gutterWidthInPixels = mostDigitsInARowLineNumber *
                                      textEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

            gutterWidthInPixels += TextEditorBase.GUTTER_PADDING_LEFT_IN_PIXELS +
                                   TextEditorBase.GUTTER_PADDING_RIGHT_IN_PIXELS;

            var widthOfBody = _widthAndHeightOfTextEditorEntirety.WidthInPixels - gutterWidthInPixels;
            
            textEditorViewModel.WidthAndHeightOfBody = new WidthAndHeightOfTextEditor
            {
                WidthInPixels = widthOfBody,
                HeightInPixels = _widthAndHeightOfTextEditorEntirety.HeightInPixels
            };
                
            {
                textEditorViewModel.ShouldMeasureDimensions = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    private async Task ForceVirtualizationInvocation()
    {
        var textEditorViewModel = ReplaceableTextEditorViewModel;

        if (textEditorViewModel is null)
            return;
        
        _virtualizationDisplay?.InvokeEntriesProviderFunc();
        _virtualizationDisplay?.ForceReadScrollPosition(textEditorViewModel.BodyElementId);
    }
    
    private async void TextEditorStatesSelectionOnSelectedValueChanged(object? sender, TextEditorBase? e)
    {
        await ForceVirtualizationInvocation();
    }

    public async Task FocusTextEditorAsync()
    {
        if (_textEditorCursorDisplay is not null)
            await _textEditorCursorDisplay.FocusAsync();
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        var safeTextEditorReference = MutableReferenceToTextEditor;
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

        if (safeTextEditorReference is null ||
            safeTextEditorViewModel is null)
        {
            return;
        }

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeTextEditorViewModel.PrimaryCursor);

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
                        safeTextEditorViewModel));
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
                        safeTextEditorViewModel.TextEditorKey,
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
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

        if (safeTextEditorReference is null ||
            safeTextEditorViewModel is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeTextEditorViewModel.PrimaryCursor);

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
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

        if (safeTextEditorReference is null ||
            safeTextEditorViewModel is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeTextEditorViewModel.PrimaryCursor);

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
            var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

            if (safeTextEditorReference is null ||
                safeTextEditorViewModel is null)
                return;

            var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeTextEditorViewModel.PrimaryCursor);

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
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

        if (safeTextEditorReference is null ||
            safeTextEditorViewModel is null)
            return (0, 0);

        RelativeCoordinatesOnClick = await JsRuntime
            .InvokeAsync<RelativeCoordinates>(
                "blazorTextEditor.getRelativePosition",
                safeTextEditorViewModel.BodyElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY);

        if (safeTextEditorViewModel.CharacterWidthAndRowHeight is null)
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

        var columnIndexDouble = positionX / 
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var columnIndexInt = (int)Math.Round(
            columnIndexDouble,
            MidpointRounding.AwayFromZero);

        var rowIndex = (int)(positionY / 
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels);

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
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;
        
        if (safeTextEditorViewModel is null ||
            safeTextEditorViewModel.CharacterWidthAndRowHeight is null ||
            safeTextEditorViewModel.WidthAndHeightOfBody is null ||
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
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels);

        var verticalTake = (int)Math.Ceiling(
            safeTextEditorViewModel.WidthAndHeightOfBody.HeightInPixels /
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels);
        
        // Vertical Padding (render some offscreen data)
        {
            verticalTake += 1;
        }
        
        // Check index boundaries
        {
            verticalStartingIndex = Math.Max(0, verticalStartingIndex);
            
            
            if (verticalStartingIndex + verticalTake >
                safeTextEditorReference.RowEndingPositions.Length)
            {
                verticalTake = safeTextEditorReference.RowEndingPositions.Length -
                               verticalStartingIndex;
            }
            
            verticalTake = Math.Max(0, verticalTake);
        }

        var horizontalStartingIndex = (int)Math.Floor(
            request.ScrollPosition.ScrollLeftInPixels /
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var horizontalTake = (int)Math.Ceiling(
            safeTextEditorViewModel.WidthAndHeightOfBody.WidthInPixels /
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels);

        var virtualizedEntries = safeTextEditorReference
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

                    var tabsOnSameRowBeforeCursor = safeTextEditorReference
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
                    safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

                var leftInPixels =
                    // do not change this to localHorizontalStartingIndex
                    horizontalStartingIndex *
                    safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

                var topInPixels =
                    index *
                    safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels;

                return new VirtualizationEntry<List<RichCharacter>>(
                    index,
                    horizontallyVirtualizedRow,
                    widthInPixels,
                    safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels,
                    leftInPixels,
                    topInPixels);
            }).ToImmutableArray();

        var totalWidth =
            safeTextEditorReference.MostCharactersOnASingleRow *
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var totalHeight =
            safeTextEditorReference.RowEndingPositions.Length *
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels;
        
        // Add vertical margin so the user can scroll beyond the final row of content
        {
            var percentOfMarginScrollHeightByPageUnit = 0.4;
            
            var marginScrollHeight =
                (safeTextEditorViewModel.WidthAndHeightOfBody?.HeightInPixels ?? 0) *
                percentOfMarginScrollHeightByPageUnit;

            totalHeight += marginScrollHeight;
        }

        var leftBoundaryWidthInPixels =
            horizontalStartingIndex *
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var leftBoundary = new VirtualizationBoundary(
            leftBoundaryWidthInPixels,
            null,
            0,
            0);

        var rightBoundaryLeftInPixels =
            leftBoundary.WidthInPixels +
            safeTextEditorViewModel.CharacterWidthAndRowHeight.CharacterWidthInPixels *
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
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels;

        var topBoundary = new VirtualizationBoundary(
            null,
            topBoundaryHeightInPixels,
            0,
            0);

        var bottomBoundaryTopInPixels =
            topBoundary.HeightInPixels +
            safeTextEditorViewModel.CharacterWidthAndRowHeight.RowHeightInPixels *
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
    
    private async Task HandleOnWheelAsync(WheelEventArgs wheelEventArgs)
    {
        var textEditorViewModel = ReplaceableTextEditorViewModel;

        if (textEditorViewModel is null)
            return;
        
        if (wheelEventArgs.ShiftKey)
        {
            await textEditorViewModel.MutateScrollHorizontalPositionByPixelsAsync(
                wheelEventArgs.DeltaY);
        }
        else
        {
            await textEditorViewModel.MutateScrollVerticalPositionByPixelsAsync(
                wheelEventArgs.DeltaY);
        }
    }
    
    private string GetGlobalHeightInPixelsStyling()
    {
        var heightInPixels = TextEditorService
            .TextEditorStates
            .GlobalTextEditorOptions
            .HeightInPixels;

        if (heightInPixels is null)
            return string.Empty;

        return $"height: {heightInPixels.Value}px;";
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