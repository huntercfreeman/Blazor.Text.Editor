using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Virtualization;

public record VirtualizationResult<T>(
        ImmutableArray<VirtualizationEntry<T>> Entries,
        VirtualizationBoundary LeftVirtualizationBoundary,
        VirtualizationBoundary RightVirtualizationBoundary,
        VirtualizationBoundary TopVirtualizationBoundary,
        VirtualizationBoundary BottomVirtualizationBoundary,
        VirtualizationScrollPosition VirtualizationScrollPosition)
    : IVirtualizationResultWithoutTypeMask;