using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Menu;

public record MenuRecord(ImmutableArray<MenuOptionRecord> MenuOptions);