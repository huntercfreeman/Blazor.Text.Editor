using System.Collections.Immutable;
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.JavaScriptObjects;
using BlazorCommon.RazorLib.Keyboard;
using BlazorCommon.RazorLib.Misc;
using BlazorCommon.RazorLib.Reactive;
using BlazorCommon.RazorLib.RenderTracker;
using BlazorCommon.RazorLib.Store.RenderTrackerCase;
using BlazorTextEditor.RazorLib.Autocomplete;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;
using BlazorTextEditor.RazorLib.Semantics;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.Store.Options;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel.InternalComponents;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.ViewModel;

public partial class TextEditorViewModelDisplay : ComponentBase, IDisposable
{
    [Inject]
    protected IState<TextEditorModelsCollection> ModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorViewModelsCollection> ViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    protected IState<TextEditorOptionsState> GlobalOptionsWrap { get; set; } = null!;
    [Inject]
    protected ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAutocompleteIndexer AutocompleteIndexer { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskQueue BackgroundTaskQueue { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorViewModelKey TextEditorViewModelKey { get; set; } = null!;
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
    public Func<TextEditorModel, ImmutableArray<TextEditorCursorSnapshot>, KeyboardEventArgs, Func<TextEditorMenuKind, bool, Task>, Task>? AfterOnKeyDownAsync { get; set; }
    /// <summary>If set to false the <see cref="TextEditorHeader"/> will NOT render above the text editor.</summary>
    [Parameter]
    public bool IncludeHeaderHelperComponent { get; set; } = true;
    /// <summary><see cref="HeaderButtonKinds"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.</summary>
    [Parameter]
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }
    /// <summary>If set to false the <see cref="TextEditorFooter"/> will NOT render below the text editor.</summary>
    [Parameter]
    public bool IncludeFooterHelperComponent { get; set; } = true;
    [Parameter]
    public bool IncludeContextMenuHelperComponent { get; set; } = true;


    private readonly IThrottle<byte> _afterOnKeyDownSyntaxHighlightingThrottle = new Throttle<byte>(TimeSpan.FromMilliseconds(750));
    private readonly IThrottle<(MouseEventArgs, bool thinksLeftMouseButtonIsDown)> _onMouseMoveThrottle = new Throttle<(MouseEventArgs, bool thinksLeftMouseButtonIsDown)>(TimeSpan.FromMilliseconds(30));
    private readonly IThrottle<(TouchEventArgs, bool thinksLeftMouseButtonIsDown)> _onTouchMoveThrottle = new Throttle<(TouchEventArgs, bool thinksLeftMouseButtonIsDown)>(TimeSpan.FromMilliseconds(30));

    private readonly Guid _textEditorHtmlElementId = Guid.NewGuid();

    private TextEditorViewModelKey _activeTextEditorViewModelKey = TextEditorViewModelKey.Empty;
    /// <summary>This accounts for one who might hold down Left Mouse Button from outside the TextEditorDisplay's content div then move their mouse over the content div while holding the Left Mouse Button down.</summary>
    private bool _thinksLeftMouseButtonIsDown;
    private bool _thinksTouchIsOccurring;
    private TouchEventArgs? _previousTouchEventArgs = null;
    private DateTime? _touchStartDateTime = null;
    private BodySection? _bodySectionComponent;
    private MeasureCharacterWidthAndRowHeight? _measureCharacterWidthAndRowHeightComponent;
    private RenderStateKey _currentViewModelRenderStateKey = RenderStateKey.Empty;
    private bool _internalStateHasChangedFlag;

    private TextEditorCursorDisplay? TextEditorCursorDisplay => _bodySectionComponent?.TextEditorCursorDisplayComponent;
    private string MeasureCharacterWidthAndRowHeightElementId => $"bte_measure-character-width-and-row-height_{_textEditorHtmlElementId}";
    private string ContentElementId => $"bte_text-editor-content_{_textEditorHtmlElementId}";
    private string ProportionalFontMeasurementsContainerElementId => $"bte_text-editor-proportional-font-measurement-container_{_textEditorHtmlElementId}";
    private string RenderTrackerDisplayName => $"{nameof(TextEditorViewModelDisplay)}_{_textEditorHtmlElementId}";

    protected override async Task OnParametersSetAsync()
    {
        var currentViewModel = GetViewModel();

        if (currentViewModel is not null &&
            _activeTextEditorViewModelKey != currentViewModel.ViewModelKey)
        {
            _activeTextEditorViewModelKey = currentViewModel.ViewModelKey;

            currentViewModel.PrimaryCursor.ShouldRevealCursor = true;
        }
        else
        {
            _activeTextEditorViewModelKey = TextEditorViewModelKey.Empty;
        }

        await base.OnParametersSetAsync();
    }
    
    protected override void OnInitialized()
    {
        var renderTrackerObject = new RenderTrackerObject(
            RenderTrackerDisplayName,
            DateTime.UtcNow.Ticks,
            "Show a text editor");

        Dispatcher.Dispatch(
            new RenderTrackerState.RegisterAction(
                renderTrackerObject));

        ModelsCollectionWrap.StateChanged += GeneralOnStateChangedEventHandler;
        ViewModelsCollectionWrap.StateChanged += GeneralOnStateChangedEventHandler;
        GlobalOptionsWrap.StateChanged += GeneralOnStateChangedEventHandler;

        base.OnInitialized();
    }

    /// <summary>Is ShouldRender() guaranteed to result in a render if I return true?</summary>
    protected override bool ShouldRender()
    {
        var renderBatch = new TextEditorRenderBatch(
            GetModel(),
            GetViewModel(),
            GetOptions());

        if (renderBatch.ViewModel is null)
            return true;
        else if (renderBatch.ViewModel.IsDirty(renderBatch.Options))
            return true;
        else if (renderBatch.ViewModel.IsDirty(renderBatch.Model))
            return true;
        else if (renderBatch.ViewModel.RenderStateKey != _currentViewModelRenderStateKey)
            return true;

        // TODO: Perhaps "return false" should be changed to "return base.ShouldRender();" for the sake of MouseEvents? Meaning to say, does "return false" break anything?
        return base.ShouldRender();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.preventDefaultOnWheelEvents",
                ContentElementId);
        }

        var renderBatch = new TextEditorRenderBatch(
            GetModel(),
            GetViewModel(),
            GetOptions());

        if (renderBatch?.ViewModel is null)
        {
            goto finalize;
        }
        else if (renderBatch.ViewModel.IsDirty(renderBatch.Options))
        {
            if (renderBatch.Options is not null)
            {
                // Tracking renders while working to fix a infinite render loop bug.
                {
                    var renderTrackerEntry = new RenderTrackerEntry(
                                        DateTime.UtcNow.Ticks,
                                        "RemeasureAsync");

                    Dispatcher.Dispatch(
                        new RenderTrackerState.AddEntryAction(
                            RenderTrackerDisplayName,
                            renderTrackerEntry));
                }

                await renderBatch.ViewModel.RemeasureAsync(
                    renderBatch.Options,
                    MeasureCharacterWidthAndRowHeightElementId,
                    _measureCharacterWidthAndRowHeightComponent?.CountOfTestCharacters ?? 0);
            }

            goto finalize;
        }
        else if (renderBatch.ViewModel.IsDirty(renderBatch.Model))
        {
            // Tracking renders while working to fix a infinite render loop bug.
            {
                var renderTrackerEntry = new RenderTrackerEntry(
                                    DateTime.UtcNow.Ticks,
                                    "CalculateVirtualizationResultAsync");

                Dispatcher.Dispatch(
                    new RenderTrackerState.AddEntryAction(
                        RenderTrackerDisplayName,
                        renderTrackerEntry));
            }

            await renderBatch.ViewModel.CalculateVirtualizationResultAsync(
                renderBatch.Model,
                null,
                false,
                CancellationToken.None);

            goto finalize;
        }
        else
        {
            if (renderBatch.ViewModel.ShouldSetFocusAfterNextRender)
            {
                renderBatch.ViewModel.ShouldSetFocusAfterNextRender = false;
                await FocusTextEditorAsync();
            }

            goto finalize;
        }

        finalize:
        _currentViewModelRenderStateKey = renderBatch?.ViewModel?.RenderStateKey ?? RenderStateKey.Empty;
        await base.OnAfterRenderAsync(firstRender);
    }

    public TextEditorModel? GetModel() => TextEditorService.ViewModel
        .FindBackingModelOrDefault(TextEditorViewModelKey);

    public TextEditorViewModel? GetViewModel() => ViewModelsCollectionWrap.Value.ViewModelsList
        .FirstOrDefault(x => x.ViewModelKey == TextEditorViewModelKey);
    
    public TextEditorOptions? GetOptions() => GlobalOptionsWrap.Value.Options;

    private async void GeneralOnStateChangedEventHandler(object? sender, EventArgs e)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "GeneralOnStateChangedEventHandler");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        await InvokeAsync(StateHasChanged);
    }

    public async Task FocusTextEditorAsync()
    {
        if (TextEditorCursorDisplay is not null)
            await TextEditorCursorDisplay.FocusAsync();
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleOnKeyDownAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        var safeRefModel = GetModel();
        var safeRefViewModel = GetViewModel();

        if (safeRefModel is null ||
            safeRefViewModel is null)
        {
            return;
        }

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeRefViewModel.PrimaryCursor);

        var cursorSnapshots = new TextEditorCursorSnapshot[]
        {
            new(primaryCursorSnapshot.UserCursor),
        }.ToImmutableArray();

        var hasSelection = TextEditorSelectionHelper
                .HasSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableSelection);
        
        var command = TextEditorService
            .OptionsWrap.Value.Options.KeymapDefinition!.Keymap.Map(
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
                await TextEditorCursorDisplay.SetFocusToActiveMenuAsync();
            }
            else
            {
                TextEditorCursor.MoveCursor(
                    keyboardEventArgs,
                    primaryCursorSnapshot.UserCursor,
                    safeRefModel);

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
                        safeRefModel,
                        cursorSnapshots,
                        ClipboardService,
                        TextEditorService,
                        safeRefViewModel));
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

                // Tracking renders while working to fix a infinite render loop bug.
                {
                    var renderTrackerEntry = new RenderTrackerEntry(
                                        DateTime.UtcNow.Ticks,
                                        "KeyboardEventAction");

                    Dispatcher.Dispatch(
                        new RenderTrackerState.AddEntryAction(
                            RenderTrackerDisplayName,
                            renderTrackerEntry));
                }

                Dispatcher.Dispatch(
                    new TextEditorModelsCollection.KeyboardEventAction(
                        safeRefViewModel.ModelKey,
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
            var textEditor = safeRefModel;
            
            // IBackgroundTaskQueue does not work well here because
            // this Task does not need to be tracked.
            _ = Task.Run(async () =>
            {
                try
                {           
                    await afterOnKeyDownAsync.Invoke(
                        textEditor,
                        cursorSnapshots,
                        keyboardEventArgs,
                        cursorDisplay.SetShouldDisplayMenuAsync);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None);
        }
    }

    private void HandleOnContextMenuAsync()
    {
        TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
    }

    private async Task HandleContentOnDoubleClickAsync(MouseEventArgs mouseEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleContentOnDoubleClickAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        var safeRefModel = GetModel();
        var safeRefViewModel = GetViewModel();

        if (safeRefModel is null ||
            safeRefViewModel is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeRefViewModel.PrimaryCursor);

        if ((mouseEventArgs.Buttons & 1) != 1 &&
            TextEditorSelectionHelper.HasSelectedText(
                primaryCursorSnapshot.ImmutableCursor.ImmutableSelection))
            // Not pressing the left mouse button
            // so assume ContextMenu is desired result.
            return;

        if (mouseEventArgs.ShiftKey)
            // Do not expand selection if user is holding shift
            return;

        var rowAndColumnIndex =
            await CalculateRowAndColumnIndex(mouseEventArgs);

        var lowerColumnIndexExpansion = safeRefModel
            .GetColumnIndexOfCharacterWithDifferingKind(
                rowAndColumnIndex.rowIndex,
                rowAndColumnIndex.columnIndex,
                true);

        lowerColumnIndexExpansion =
            lowerColumnIndexExpansion == -1
                ? 0
                : lowerColumnIndexExpansion;

        var higherColumnIndexExpansion = safeRefModel
            .GetColumnIndexOfCharacterWithDifferingKind(
                rowAndColumnIndex.rowIndex,
                rowAndColumnIndex.columnIndex,
                false);

        higherColumnIndexExpansion =
            higherColumnIndexExpansion == -1
                ? safeRefModel.GetLengthOfRow(
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
            var cursorPositionOfHigherExpansion = safeRefModel
                .GetPositionIndex(
                    rowAndColumnIndex.rowIndex,
                    higherColumnIndexExpansion);

            primaryCursorSnapshot
                    .UserCursor.Selection.EndingPositionIndex =
                cursorPositionOfHigherExpansion;
        }

        // Set text selection anchor to lower expansion
        {
            var cursorPositionOfLowerExpansion = safeRefModel
                .GetPositionIndex(
                    rowAndColumnIndex.rowIndex,
                    lowerColumnIndexExpansion);

            primaryCursorSnapshot
                    .UserCursor.Selection.AnchorPositionIndex =
                cursorPositionOfLowerExpansion;
        }
    }

    private async Task HandleContentOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleContentOnMouseDownAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        var safeRefModel = GetModel();
        var safeRefViewModel = GetViewModel();

        if (safeRefModel is null ||
            safeRefViewModel is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeRefViewModel.PrimaryCursor);

        if ((mouseEventArgs.Buttons & 1) != 1 &&
            TextEditorSelectionHelper.HasSelectedText(
                primaryCursorSnapshot.ImmutableCursor.ImmutableSelection))
            // Not pressing the left mouse button
            // so assume ContextMenu is desired result.
            return;

        TextEditorCursorDisplay?.SetShouldDisplayMenuAsync(
            TextEditorMenuKind.None,
            false);

        var rowAndColumnIndex =
            await CalculateRowAndColumnIndex(mouseEventArgs);

        primaryCursorSnapshot.UserCursor.IndexCoordinates =
            (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
            rowAndColumnIndex.columnIndex;

        TextEditorCursorDisplay?.PauseBlinkAnimation();

        var cursorPositionIndex = safeRefModel
            .GetCursorPositionIndex(
                new TextEditorCursor(rowAndColumnIndex, false));

        if (mouseEventArgs.ShiftKey)
        {
            if (!TextEditorSelectionHelper.HasSelectedText(
                    primaryCursorSnapshot.ImmutableCursor.ImmutableSelection))
            {
                // If user does not yet have a selection
                // then place the text selection anchor were they were

                var cursorPositionPriorToMovementOccurring = safeRefModel
                    .GetPositionIndex(
                        primaryCursorSnapshot.ImmutableCursor.RowIndex,
                        primaryCursorSnapshot.ImmutableCursor.ColumnIndex);

                primaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                    cursorPositionPriorToMovementOccurring;
            }

            // If user ALREADY has a selection
            // then do not modify the text selection anchor
        }
        else
        {
            primaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                cursorPositionIndex;
        }

        primaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
            cursorPositionIndex;

        _thinksLeftMouseButtonIsDown = true;
    }

    /// <summary>OnMouseUp is unnecessary</summary>
    private async Task HandleContentOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            // TODO: Temporarily commenting out this render tracking. It happens so much that I pretty much cannot see anything other than it.
            //var renderTrackerEntry = new RenderTrackerEntry(
            //                    DateTime.UtcNow.Ticks,
            //                    "HandleContentOnMouseMoveAsync");

            //Dispatcher.Dispatch(
            //    new RenderTrackerState.AddEntryAction(
            //        RenderTrackerDisplayName,
            //        renderTrackerEntry));
        }

        var localThinksLeftMouseButtonIsDown = _thinksLeftMouseButtonIsDown;
        
        if (!_thinksLeftMouseButtonIsDown)
            return;
        
        var mostRecentEventArgs = await _onMouseMoveThrottle.FireAsync(
            (mouseEventArgs, localThinksLeftMouseButtonIsDown),
            CancellationToken.None);

        if (mostRecentEventArgs.isCancellationRequested)
            return;

        localThinksLeftMouseButtonIsDown = mostRecentEventArgs.tEventArgs.thinksLeftMouseButtonIsDown;
        mouseEventArgs = mostRecentEventArgs.tEventArgs.Item1;
        
        var safeRefModel = GetModel();
        var safeRefViewModel = GetViewModel();

        if (safeRefModel is null ||
            safeRefViewModel is null)
            return;

        var primaryCursorSnapshot = new TextEditorCursorSnapshot(safeRefViewModel.PrimaryCursor);

        // Buttons is a bit flag
        // '& 1' gets if left mouse button is held
        if (localThinksLeftMouseButtonIsDown &&
            (mouseEventArgs.Buttons & 1) == 1)
        {
            var rowAndColumnIndex =
                await CalculateRowAndColumnIndex(mouseEventArgs);

            primaryCursorSnapshot.UserCursor.IndexCoordinates =
                (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
            primaryCursorSnapshot.UserCursor.PreferredColumnIndex =
                rowAndColumnIndex.columnIndex;

            TextEditorCursorDisplay?.PauseBlinkAnimation();

            primaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                safeRefModel
                    .GetCursorPositionIndex(
                        new TextEditorCursor(rowAndColumnIndex, false));
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;
        }
    }

    private async Task<(int rowIndex, int columnIndex)> CalculateRowAndColumnIndex(
        MouseEventArgs mouseEventArgs)
    {
        var safeRefModel = GetModel();
        var safeRefViewModel = GetViewModel();
        var globalTextEditorOptions = TextEditorService.OptionsWrap.Value.Options;

        if (safeRefModel is null ||
            safeRefViewModel is null)
            return (0, 0);

        var relativeCoordinatesOnClick = await JsRuntime
            .InvokeAsync<RelativeCoordinates>(
                "blazorTextEditor.getRelativePosition",
                safeRefViewModel.BodyElementId,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY);

        var positionX = relativeCoordinatesOnClick.RelativeX;
        var positionY = relativeCoordinatesOnClick.RelativeY;

        // Scroll position offset
        {
            positionX += relativeCoordinatesOnClick.RelativeScrollLeft;
            positionY += relativeCoordinatesOnClick.RelativeScrollTop;
        }
        
        var rowIndex = (int)(positionY / 
                             safeRefViewModel.VirtualizationResult.CharacterWidthAndRowHeight.RowHeightInPixels);

        rowIndex = rowIndex > safeRefModel.RowCount - 1
            ? safeRefModel.RowCount - 1
            : rowIndex;

        int columnIndexInt;
        
        if (!globalTextEditorOptions.UseMonospaceOptimizations)
        {
            var guid = Guid.NewGuid();
            
            columnIndexInt = await JsRuntime.InvokeAsync<int>(
                "blazorTextEditor.calculateProportionalColumnIndex",
                ProportionalFontMeasurementsContainerElementId,
                $"bte_proportional-font-measurement-parent_{_textEditorHtmlElementId}_{guid}",
                $"bte_proportional-font-measurement-cursor_{_textEditorHtmlElementId}_{guid}",
                positionX,
                safeRefViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels,
                safeRefModel.GetTextOnRow(rowIndex));

            if (columnIndexInt == -1)
            {
                var columnIndexDouble = positionX / 
                                        safeRefViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

                columnIndexInt = (int)Math.Round(
                    columnIndexDouble,
                    MidpointRounding.AwayFromZero);
            }
        }
        else
        {
            var columnIndexDouble = positionX / 
                                    safeRefViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

            columnIndexInt = (int)Math.Round(
                columnIndexDouble,
                MidpointRounding.AwayFromZero);
        }

        var lengthOfRow = safeRefModel.GetLengthOfRow(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor =
                columnIndexInt > lengthOfRow
                    ? lengthOfRow
                    : columnIndexInt;

            var tabsOnSameRowBeforeCursor = safeRefModel
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex,
                    parameterForGetTabsCountOnSameRowBeforeCursor);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            columnIndexInt -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
        }

        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);

        return (rowIndex, columnIndexInt);
    }

    /// <summary>The default <see cref="AfterOnKeyDownAsync"/> will provide syntax highlighting, and autocomplete.<br/><br/>The syntax highlighting occurs on ';', whitespace, paste, undo, redo<br/><br/>The autocomplete occurs on LetterOrDigit typed or { Ctrl + Space }. Furthermore, the autocomplete is done via <see cref="IAutocompleteService"/> and the one can provide their own implementation when registering the BlazorTextEditor services using <see cref="BlazorTextEditorOptions.AutocompleteServiceFactory"/></summary>
    public async Task HandleAfterOnKeyDownAsync(
        TextEditorModel textEditor,
        ImmutableArray<TextEditorCursorSnapshot> cursorSnapshots,
        KeyboardEventArgs keyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task> setTextEditorMenuKind)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleAfterOnKeyDownAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

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
            var mostRecentEventArgs = await _afterOnKeyDownSyntaxHighlightingThrottle
                .FireAsync(
                    0,
                    CancellationToken.None);

            if (mostRecentEventArgs.isCancellationRequested)
                return;

            // The TextEditorModel may have been changed by the time this logic is ran and
            // thus the local variable must be updated accordingly.
            var temporaryTextEditor = GetModel();

            if (temporaryTextEditor is not null)
            {
                textEditor = temporaryTextEditor;

                await textEditor.ApplySyntaxHighlightingAsync();

                ChangeLastPresentationLayer();

                // Tracking renders while working to fix a infinite render loop bug.
                {
                    var renderTrackerEntry = new RenderTrackerEntry(
                                        DateTime.UtcNow.Ticks,
                                        "ApplySyntaxHighlightingAsync");

                    Dispatcher.Dispatch(
                        new RenderTrackerState.AddEntryAction(
                            RenderTrackerDisplayName,
                            renderTrackerEntry));
                }
            }
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
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleOnWheelAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        var textEditorViewModel = GetViewModel();

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
            .OptionsWrap
            .Value
            .Options
            .TextEditorHeightInPixels;

        if (heightInPixels is null)
            return string.Empty;

        var heightInPixelsInvariantCulture = heightInPixels.Value
            .ToCssValue();
        
        return $"height: {heightInPixelsInvariantCulture}px;";
    }
    
    private Task HandleOnTouchStartAsync(TouchEventArgs touchEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleOnTouchStartAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        _touchStartDateTime = DateTime.UtcNow;
        
        _previousTouchEventArgs = touchEventArgs;
        _thinksTouchIsOccurring = true;

        return Task.CompletedTask;
    }
    
    private async Task HandleOnTouchMoveAsync(TouchEventArgs touchEventArgs)
    {
        // Tracking renders while working to fix a infinite render loop bug.
        {
            var renderTrackerEntry = new RenderTrackerEntry(
                                DateTime.UtcNow.Ticks,
                                "HandleOnTouchMoveAsync");

            Dispatcher.Dispatch(
                new RenderTrackerState.AddEntryAction(
                    RenderTrackerDisplayName,
                    renderTrackerEntry));
        }

        var localThinksTouchIsOccurring = _thinksTouchIsOccurring;
        
        if (!_thinksTouchIsOccurring)
            return;
        
        var mostRecentEventArgs = await _onTouchMoveThrottle.FireAsync(
            (touchEventArgs, localThinksTouchIsOccurring),
            CancellationToken.None);

        if (mostRecentEventArgs.isCancellationRequested)
            return;

        localThinksTouchIsOccurring = mostRecentEventArgs.tEventArgs.thinksLeftMouseButtonIsDown;
        touchEventArgs = mostRecentEventArgs.tEventArgs.Item1;

        var previousTouchPoint = _previousTouchEventArgs?.ChangedTouches
            .FirstOrDefault(x => x.Identifier == 0);
        
        var currentTouchPoint = touchEventArgs.ChangedTouches
            .FirstOrDefault(x => x.Identifier == 0);

        if (previousTouchPoint is null || currentTouchPoint is null)
            return;

        var viewModel = GetViewModel();

        if (viewModel is null)
            return;
        
        // Natural scrolling for touch devices
        var diffX = previousTouchPoint.ClientX - currentTouchPoint.ClientX;
        var diffY = previousTouchPoint.ClientY - currentTouchPoint.ClientY;

        await viewModel.MutateScrollHorizontalPositionByPixelsAsync(diffX);
        await viewModel.MutateScrollVerticalPositionByPixelsAsync(diffY);

        _previousTouchEventArgs = touchEventArgs;
    }
    
    private async Task ClearTouchAsync(TouchEventArgs touchEventArgs)
    {
        var rememberStartTouchEventArgs = _previousTouchEventArgs;
        
        _thinksTouchIsOccurring = false;
        _previousTouchEventArgs = null;

        var clearTouchDateTime = DateTime.UtcNow;

        var touchTimespan = clearTouchDateTime - _touchStartDateTime;

        if (touchTimespan is null)
            return;
        
        if (touchTimespan.Value.TotalMilliseconds < 200)
        {
            var startTouchPoint = rememberStartTouchEventArgs?.ChangedTouches
                .FirstOrDefault(x => x.Identifier == 0);

            if (startTouchPoint is null)
                return;
            
            await HandleContentOnMouseDownAsync(new MouseEventArgs
            {
                Buttons = 1,
                ClientX = startTouchPoint.ClientX,
                ClientY = startTouchPoint.ClientY,
            });
        }
    }

    private void ChangeLastPresentationLayer()
    {
        var viewModel = GetViewModel();

        if (viewModel is null)
            return;

        TextEditorService.ViewModel.With(
            viewModel.ViewModelKey,
            inViewModel =>
            {
                var outPresentationLayer = inViewModel.FirstPresentationLayer;

                var inPresentationModel = outPresentationLayer
                    .FirstOrDefault(x =>
                        x.TextEditorPresentationKey == SemanticFacts.PresentationKey);

                if (inPresentationModel is null)
                {
                    inPresentationModel = SemanticFacts.EmptyPresentationModel;

                    outPresentationLayer = outPresentationLayer.Add(
                        inPresentationModel);
                }

                var model = TextEditorService.ViewModel
                    .FindBackingModelOrDefault(viewModel.ViewModelKey);

                var outPresentationModel = inPresentationModel with
                {
                    TextEditorTextSpans = model?.SemanticModel?.DiagnosticTextSpans
                        ?? ImmutableList<TextEditorTextSpan>.Empty
                };

                outPresentationLayer = outPresentationLayer.Replace(
                    inPresentationModel,
                    outPresentationModel);

                return inViewModel with
                {
                    FirstPresentationLayer = outPresentationLayer,
                    RenderStateKey = RenderStateKey.NewRenderStateKey()
                };
            });
    }

    public void Dispose()
    {
        ModelsCollectionWrap.StateChanged -= GeneralOnStateChangedEventHandler;
        ViewModelsCollectionWrap.StateChanged -= GeneralOnStateChangedEventHandler;
        GlobalOptionsWrap.StateChanged -= GeneralOnStateChangedEventHandler;

        Dispatcher.Dispatch(
            new RenderTrackerState.DisposeAction(
                RenderTrackerDisplayName));
    }
}