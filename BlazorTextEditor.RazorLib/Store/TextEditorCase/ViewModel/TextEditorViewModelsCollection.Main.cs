﻿using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;

[FeatureState]
public partial class TextEditorViewModelsCollection
{
    public TextEditorViewModelsCollection()
    {
    }

    /// <summary>
    /// Keep the <see cref="TextEditorViewModelsCollection"/> as a class
    /// as to avoid record value comparisons when Fluxor checks
    /// if the <see cref="FeatureStateAttribute"/> has been replaced.
    /// </summary>
    public ImmutableList<TextEditorViewModel> ViewModelsList { get; init; } = 
        ImmutableList<TextEditorViewModel>.Empty;
}