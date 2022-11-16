namespace BlazorTextEditor.RazorLib.Virtualization;

public record VirtualizationRequest(
    VirtualizationScrollPosition ScrollPosition,
    CancellationToken CancellationToken);