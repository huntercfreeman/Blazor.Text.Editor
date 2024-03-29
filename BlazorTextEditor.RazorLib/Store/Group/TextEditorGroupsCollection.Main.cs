﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Group;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Group;

/// <summary>
/// Keep the <see cref="TextEditorGroupsCollection"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorGroupsCollection
{
    public TextEditorGroupsCollection()
    {
        GroupsList = ImmutableList<TextEditorGroup>.Empty; 
    }

    public ImmutableList<TextEditorGroup> GroupsList { get; init; }
}