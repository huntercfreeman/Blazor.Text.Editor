using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Virtualization;

public partial class VirtualizationDisplay<T> : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<VirtualizationRequest, VirtualizationResult<T>?> EntriesProviderFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<VirtualizationResult<T>> ChildContent { get; set; } = null!;

    [Parameter]
    public bool UseHorizontalVirtualization { get; set; } = true;
    [Parameter]
    public bool UseVerticalVirtualization { get; set; } = true;
    
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    private VirtualizationRequest _request = null!;

    private VirtualizationResult<T> _result = new(
        ImmutableArray<VirtualizationEntry<T>>.Empty,
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new(0, 0, 0, 0));

    private ElementReference _scrollableParentFinder;

    private CancellationTokenSource _scrollEventCancellationTokenSource = new();

    private string LeftVirtualizationBoundaryDisplayId =>
        $"bte_left-virtualization-boundary-display-{_intersectionObserverMapKey}";

    private string RightVirtualizationBoundaryDisplayId =>
        $"bte_right-virtualization-boundary-display-{_intersectionObserverMapKey}";

    private string TopVirtualizationBoundaryDisplayId =>
        $"bte_top-virtualization-boundary-display-{_intersectionObserverMapKey}";

    private string BottomVirtualizationBoundaryDisplayId =>
        $"bte_bottom-virtualization-boundary-display-{_intersectionObserverMapKey}";

    protected override void OnInitialized()
    {
        _scrollEventCancellationTokenSource.Cancel();
        _scrollEventCancellationTokenSource = new CancellationTokenSource();
        
        _request = new(
            new VirtualizationScrollPosition(0, 0, 0, 0),
            _scrollEventCancellationTokenSource.Token);
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var boundaryIds = new List<object>();

            if (UseHorizontalVirtualization)
            {
                boundaryIds.AddRange(new[]
                {
                    LeftVirtualizationBoundaryDisplayId,
                    RightVirtualizationBoundaryDisplayId,
                });
            }

            if (UseVerticalVirtualization)
            {
                boundaryIds.AddRange(new[]
                {
                    TopVirtualizationBoundaryDisplayId,
                    BottomVirtualizationBoundaryDisplayId,
                });
            }

            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.initializeVirtualizationIntersectionObserver",
                _intersectionObserverMapKey.ToString(),
                DotNetObjectReference.Create(this),
                _scrollableParentFinder,
                boundaryIds);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public Task OnScrollEventAsync(VirtualizationScrollPosition scrollPosition)
    {
        _scrollEventCancellationTokenSource.Cancel();
        _scrollEventCancellationTokenSource = new CancellationTokenSource();

        _request = new VirtualizationRequest(
            scrollPosition,
            _scrollEventCancellationTokenSource.Token);

        InvokeEntriesProviderFunc();
        return Task.CompletedTask;
    }

    public void InvokeEntriesProviderFunc()
    {
        var localResult = EntriesProviderFunc.Invoke(_request);

        if (localResult is not null)
        {
            _result = localResult;

            InvokeAsync(StateHasChanged);
        }
    }
    
    /// <summary>
    /// BUG: Introduced after changing from overflow: auto to overflow: hidden
    /// <br/><br/>
    /// get new request force from virtualization request is not being updated
    /// overflow-hidden results in width: 100% no longer filling the viewable AND scrollable area
    /// instead only the initial location of the viewable area has the width of virtualization correct
    /// as the scrollbar width/height is no longer being included.
    /// Make a force reload virtualization method
    /// </summary>
    public async Task ForceReadScrollPosition(string elementId)
    {
        var scrollPosition = await JsRuntime.InvokeAsync<VirtualizationScrollPosition>(
            "blazorTextEditor.getScrollPosition",
            elementId);

        await OnScrollEventAsync(scrollPosition);
    }
    
    public void Dispose()
    {
        _scrollEventCancellationTokenSource.Cancel();

        _ = Task.Run(async () =>
            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.disposeVirtualizationIntersectionObserver",
                _intersectionObserverMapKey.ToString()));
    }
}