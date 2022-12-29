﻿using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.HelperComponents;

public partial class TextEditorInputKeymap : FluxorComponent
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;
    
    private void SelectedKeymapChanged(ChangeEventArgs changeEventArgs)
    {
        var allKeymapDefinitions = KeymapFacts.AllKeymapDefinitions;
        
        var chosenKeymapGuidString = changeEventArgs.Value?.ToString() ?? string.Empty;

        if (Guid.TryParse(chosenKeymapGuidString, 
                out var chosenKeymapKeyGuid))
        {
            var chosenKeymapKey = new KeymapKey(chosenKeymapKeyGuid);

            var foundKeymap = allKeymapDefinitions
                .FirstOrDefault(x => x.KeymapKey == chosenKeymapKey);
            
            if (foundKeymap is not null)
                TextEditorService.SetKeymap(foundKeymap);
        }
        else
        {
            TextEditorService.SetKeymap(KeymapFacts.DefaultKeymapDefinition);
        }
    }
}