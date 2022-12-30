using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Keymap;

public static class KeymapFacts
{
    public static readonly KeymapDefinition DefaultKeymapDefinition = new(
        new KeymapKey(Guid.Parse("4aaca759-c2c7-4e6f-9d9f-f3d17172df16")),
        "Default",
        new TextEditorKeymapDefault());

    public static readonly KeymapDefinition VimKeymapDefinition = new(
        new KeymapKey(Guid.Parse("d2122a7a-5a88-4d31-af20-5486a36e9c0c")),
        "Vim",
        new TextEditorKeymapVim());

    public static ImmutableArray<KeymapDefinition> AllKeymapDefinitions =>
        new KeymapDefinition[]
        {
            DefaultKeymapDefinition,
            VimKeymapDefinition
        }.ToImmutableArray();
}