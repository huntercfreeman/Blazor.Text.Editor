using System.Collections.Immutable;
using BlazorCommon.RazorLib.JavaScriptObjects;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Measurement;

namespace BlazorTextEditor.RazorLib.Virtualization;

public record VirtualizationResult<T>(
        ImmutableArray<VirtualizationEntry<T>> Entries,
        VirtualizationBoundary LeftVirtualizationBoundary,
        VirtualizationBoundary RightVirtualizationBoundary,
        VirtualizationBoundary TopVirtualizationBoundary,
        VirtualizationBoundary BottomVirtualizationBoundary,
        ElementMeasurementsInPixels ElementMeasurementsInPixels,
        CharacterWidthAndRowHeight CharacterWidthAndRowHeight,
        bool HasValidVirtualizationResult)
    : IVirtualizationResultWithoutTypeMask
{
    public static VirtualizationResult<List<RichCharacter>> GetEmptyRichCharacters() => new VirtualizationResult<List<RichCharacter>>(
        ImmutableArray<VirtualizationEntry<List<RichCharacter>>>.Empty,
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new ElementMeasurementsInPixels(0, 0, 0, 0, 0, 0, 0, CancellationToken.None),
        new CharacterWidthAndRowHeight(0, 0),
        false);
}