using System.Collections.Immutable;
using System.Text;
using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Misc;
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
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;

    [Parameter]
    public string WrapperStyleCssString { get; set; } = string.Empty;
    [Parameter]
    public string WrapperClassCssString { get; set; } = string.Empty;
    [Parameter]
    public string TextEditorStyleCssString { get; set; } = string.Empty;
    [Parameter]
    public string TextEditorClassCssString { get; set; } = string.Empty;
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;
    [Parameter]
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    [Parameter]
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    /// <summary>If left null, the default <see cref="HandleAfterOnKeyDownAsync"/> will be used.</summary>
    [Parameter]
    public Func<TextEditorBase, ImmutableArray<TextEditorCursorSnapshot>, KeyboardEventArgs, Func<TextEditorMenuKind, bool, Task>, Task>? AfterOnKeyDownAsync { get; set; }
    /// <summary>If set to false the <see cref="TextEditorHeader"/> will NOT render above the text editor.</summary>
    [Parameter]
    public bool IncludeHeaderHelperComponent { get; set; } = true;
    /// <summary><see cref="HeaderButtonKinds"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.</summary>
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }
    /// <summary>If set to false the <see cref="TextEditorFooter"/> will NOT render below the text editor.</summary>
    [Parameter]
    public bool IncludeFooterHelperComponent { get; set; } = true;

    private readonly SemaphoreSlim _afterOnKeyDownSyntaxHighlightingSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _afterOnKeyDownSyntaxHighlightingDelay = TimeSpan.FromMilliseconds(750);
    private int _skippedSyntaxHighlightingEventCount;

    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);

    private int? _previousGlobalFontSizeInPixels;
    private TextEditorOptions? _previousGlobalTextEditorOptions;

    private TextEditorKey? _previousTextEditorKey;
    private TextEditorViewModelKey? _previousTextEditorViewModelKey = TextEditorViewModelKey.Empty;
    private ElementReference _textEditorDisplayElementReference;
    
    /// <summary>
    /// Accounts for one who might hold down Left Mouse Button from outside the TextEditorDisplay's content div
    /// then move their mouse over the content div while holding the Left Mouse Button down.
    /// </summary>
    private bool _thinksLeftMouseButtonIsDown;

    private Guid _componentHtmlElementId = Guid.NewGuid();
    private WidthAndHeightOfTextEditor? _widthAndHeightOfTextEditorEntirety;
    private BodySection? _bodySection;
    private CancellationTokenSource _textEditorBaseChangedCancellationTokenSource = new();
    private int _rerenderCount;
    private bool _disposed;

    private TextEditorCursorDisplay? TextEditorCursorDisplay => _bodySection?.TextEditorCursorDisplay;
    private MeasureCharacterWidthAndRowHeight? MeasureCharacterWidthAndRowHeightComponent => 
        _bodySection?.MeasureCharacterWidthAndRowHeightComponent;
    
    private string MeasureCharacterWidthAndRowHeightElementId =>
        $"bte_measure-character-width-and-row-height_{_componentHtmlElementId}";
    
    private string ContentElementId =>
        $"bte_text-editor-content_{_componentHtmlElementId}";

    public RelativeCoordinates? RelativeCoordinatesOnClick { get; private set; }
    
    protected override async Task OnParametersSetAsync()
    {
        var safeTextEditorViewModel = ReplaceableTextEditorViewModel;

        var currentGlobalFontSizeInPixels = TextEditorService
            .TextEditorStates
            .GlobalTextEditorOptions
            .FontSizeInPixels!
            .Value;

        var dirtyGlobalFontSizeInPixels =
            _previousGlobalFontSizeInPixels is null ||
            _previousGlobalFontSizeInPixels != currentGlobalFontSizeInPixels;

        if (safeTextEditorViewModel is not null)
        {
            if (dirtyGlobalFontSizeInPixels)
            {
                _previousGlobalFontSizeInPixels = currentGlobalFontSizeInPixels;

                TextEditorService.SetViewModelWith(
                    TextEditorViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        ShouldMeasureDimensions = true,
                        TextEditorRenderStateKey = TextEditorRenderStateKey.NewTextEditorRenderStateKey()
                    });
            }
        }
        
        if (safeTextEditorViewModel is not null &&
            _previousTextEditorViewModelKey != TextEditorViewModelKey)
        {
            _previousTextEditorViewModelKey = TextEditorViewModelKey;

            safeTextEditorViewModel.PrimaryCursor.ShouldRevealCursor = true;
            
            await safeTextEditorViewModel.CalculateVirtualizationResultAsync(
                null, 
                CancellationToken.None);
        }

        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        TextEditorStatesWrap.StateChanged += TextEditorStatesWrapOnStateChanged;
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _rerenderCount++;
        
        var textEditorViewModel = ReplaceableTextEditorViewModel;
        
        if (firstRender && 
            textEditorViewModel is not null)
        {
            await textEditorViewModel.CalculateVirtualizationResultAsync(
                null, 
                CancellationToken.None);

            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.preventDefaultOnWheelEvents",
                ContentElementId);
        }

        if (textEditorViewModel is not null && 
            textEditorViewModel.ShouldMeasureDimensions)
        {
            var characterWidthAndRowHeight = await JsRuntime
                .InvokeAsync<CharacterWidthAndRowHeight>(
                    "blazorTextEditor.measureCharacterWidthAndRowHeight",
                    MeasureCharacterWidthAndRowHeightElementId,
                    MeasureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0);
                
            TextEditorService.SetViewModelWith(
                TextEditorViewModelKey,
                previousViewModel => previousViewModel with
                {
                    ShouldMeasureDimensions = false,
                    VirtualizationResult = previousViewModel.VirtualizationResult with
                    {
                        CharacterWidthAndRowHeight = characterWidthAndRowHeight,
                    }
                });

            // TextEditorService.SetViewModelWith() changed the underlying TextEditorViewModel and
            // thus the local variable must be updated accordingly.
            textEditorViewModel = ReplaceableTextEditorViewModel;

            if (textEditorViewModel is not null)
            {
                await textEditorViewModel.CalculateVirtualizationResultAsync(
                    null,
                    CancellationToken.None);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    
    // TODO: When the underlying "TextEditorBase" of a "TextEditorViewModel" changes. How does one efficiently rerender the "TextEditorViewModelDisplay". The issue I am thinking of is that one would have to recalculate the VirtualizationResult as the underlying contents changed. Is recalculating the VirtualizationResult the only way?
    private async void TextEditorStatesWrapOnStateChanged(object? sender, EventArgs e)
    {
        var viewModel = ReplaceableTextEditorViewModel;

        if (viewModel is not null)
        {
            _textEditorBaseChangedCancellationTokenSource.Cancel();
            _textEditorBaseChangedCancellationTokenSource = new CancellationTokenSource();
            
            await viewModel.CalculateVirtualizationResultAsync(
                viewModel.VirtualizationResult.ElementMeasurementsInPixels,
                _textEditorBaseChangedCancellationTokenSource.Token);
        }
    }
    
    public async Task FocusTextEditorAsync()
    {
        if (TextEditorCursorDisplay is not null)
            await TextEditorCursorDisplay.FocusAsync();
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

        var hasSelection = TextEditorSelectionHelper
                .HasSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableTextEditorSelection);
        
        var command = TextEditorStatesWrap.Value.GlobalTextEditorOptions.KeymapDefinition!.Keymap.Map(
            keyboardEventArgs,
            hasSelection);
        
        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code &&
            keyboardEventArgs.ShiftKey)
        {
            command = TextEditorCommandDefaultFacts.NewLineBelow;
        }

        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key) && 
            command is null)
        {
            if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keyboardEventArgs.Key ||
                 KeyboardKeyFacts.MovementKeys.ARROW_UP == keyboardEventArgs.Key) &&
                TextEditorCursorDisplay is not null &&
                TextEditorCursorDisplay.TextEditorMenuKind ==
                TextEditorMenuKind.AutoCompleteMenu)
            {
                TextEditorCursorDisplay.SetFocusToActiveMenu();
            }
            else
            {
                TextEditorCursor.MoveCursor(
                    keyboardEventArgs,
                    primaryCursorSnapshot.UserCursor,
                    safeTextEditorReference);

                TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
            }
        }
        else if (KeyboardKeyFacts.CheckIsContextMenuEvent(keyboardEventArgs))
        {
            TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
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
                        TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
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

        var cursorDisplay = TextEditorCursorDisplay;

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
        TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
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

        TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(
            TextEditorMenuKind.None,
            false);

        var rowAndColumnIndex =
            await DetermineRowAndColumnIndex(mouseEventArgs);

        primaryCursorSnapshot.UserCursor.IndexCoordinates =
            (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
            rowAndColumnIndex.columnIndex;

        TextEditorCursorDisplay?.PauseBlinkAnimation();

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

                TextEditorCursorDisplay?.PauseBlinkAnimation();

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

        var positionX = RelativeCoordinatesOnClick.RelativeX;
        var positionY = RelativeCoordinatesOnClick.RelativeY;

        // Scroll position offset
        {
            positionX += RelativeCoordinatesOnClick.RelativeScrollLeft;
            positionY += RelativeCoordinatesOnClick.RelativeScrollTop;
        }

        var columnIndexDouble = positionX / 
            safeTextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        var columnIndexInt = (int)Math.Round(
            columnIndexDouble,
            MidpointRounding.AwayFromZero);

        var rowIndex = (int)(positionY / 
            safeTextEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels);

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

        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);

        return (rowIndex, columnIndexInt);
    }

    /// <summary>
    /// The default <see cref="AfterOnKeyDownAsync"/> will provide
    /// syntax highlighting, and autocomplete.
    /// <br/><br/>
    /// The syntax highlighting occurs on ';', whitespace, paste, undo, redo
    /// <br/><br/>
    /// The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }.
    /// Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/>
    /// and the one can provide their own implementation when registering the
    /// BlazorTextEditor services using <see cref="TextEditorServiceOptions.AutocompleteServiceFactory"/>
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
                    // The TextEditorBase may have been changed by the time this logic is ran and
                    // thus the local variable must be updated accordingly.
                    textEditor = MutableReferenceToTextEditor;

                    if (textEditor is not null)
                    {
                        await textEditor.ApplySyntaxHighlightingAsync();

                        await InvokeAsync(StateHasChanged);

                        await Task.Delay(_afterOnKeyDownSyntaxHighlightingDelay);
                    }
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
               (keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "s") ||
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

        var heightInPixelsInvariantCulture = heightInPixels.Value
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        
        return $"height: {heightInPixelsInvariantCulture}px;";
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
    
        if (disposing)
        {
            TextEditorStatesWrap.StateChanged -= TextEditorStatesWrapOnStateChanged;
            _textEditorBaseChangedCancellationTokenSource.Cancel();
        }
    
        _disposed = true;
    }
}