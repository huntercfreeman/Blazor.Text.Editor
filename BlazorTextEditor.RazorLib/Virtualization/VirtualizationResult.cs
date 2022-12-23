using System.Collections.Immutable;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Measurement;

namespace BlazorTextEditor.RazorLib.Virtualization;

public record VirtualizationResult<T>(
        ImmutableArray<VirtualizationEntry<T>> Entries,
        VirtualizationBoundary LeftVirtualizationBoundary,
        VirtualizationBoundary RightVirtualizationBoundary,
        VirtualizationBoundary TopVirtualizationBoundary,
        VirtualizationBoundary BottomVirtualizationBoundary,
        ElementMeasurementsInPixels ElementMeasurementsInPixels,
        CharacterWidthAndRowHeight CharacterWidthAndRowHeight)
    : IVirtualizationResultWithoutTypeMask;