using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.Virtualization;

/// <summary>
/// Goal of this component is not to do the rendering of the items but
/// instead ONLY the rendering of the boundaries.
/// <br/><br/>
/// As of this comment (2022-12-22) the JavaScript Intersection Observer logic was removed
/// as well. It appears to all be done through using C# to calculate whether a calculation
/// of the <see cref="VirtualizationResult{T}"/> is necessary. Adding proportional fonts
/// (variable-width fonts) will break this however and the JavaScript Intersection Observer
/// will need to be added back in just copy and paste from git history.
/// </summary>
public partial class VirtualizationDisplay : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public IVirtualizationResultWithoutTypeMask VirtualizationResultWithoutTypeMask { get; set; } = null!;

    [Parameter]
    public bool UseHorizontalVirtualization { get; set; } = true;
    [Parameter]
    public bool UseVerticalVirtualization { get; set; } = true;
    
    private readonly Guid _virtualizationDisplayGuid = Guid.NewGuid();

    private string LeftBoundaryElementId =>
        $"bte_left-virtualization-boundary-display-{_virtualizationDisplayGuid}";

    private string RightBoundaryElementId =>
        $"bte_right-virtualization-boundary-display-{_virtualizationDisplayGuid}";

    private string TopBoundaryElementId =>
        $"bte_top-virtualization-boundary-display-{_virtualizationDisplayGuid}";

    private string BottomBoundaryElementId =>
        $"bte_bottom-virtualization-boundary-display-{_virtualizationDisplayGuid}";
}