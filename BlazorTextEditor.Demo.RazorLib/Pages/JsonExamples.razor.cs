﻿using BlazorTextEditor.Demo.ClassLib.TestDataFolder;
using BlazorTextEditor.Demo.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Demo.RazorLib.Pages;

public partial class JsonExamples : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorGroupKey JsonTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();
    
    private static readonly TextEditorViewModelKey JsonTextEditorViewModelKeyOne = TextEditorViewModelKey.NewTextEditorViewModelKey();
    private static readonly TextEditorViewModelKey JsonTextEditorViewModelKeyTwo = TextEditorViewModelKey.NewTextEditorViewModelKey();
    
    protected override void OnInitialized()
    {
        TextEditorService.RegisterGroup(JsonTextEditorGroupKey);
        
        TextEditorService.RegisterJsonTextEditor(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorKey,
            nameof(JsonExamples) + "_1",
            TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS);
        
        TextEditorService.RegisterJsonTextEditor(
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorKey,
            nameof(JsonExamples) + "_2",
            TestData.Json.EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL);
        
        TextEditorService.RegisterViewModel(
            TextEditorFacts.Json.JsonLaunchSettingsTextEditorKey,
            JsonTextEditorViewModelKeyOne);
        
        TextEditorService.RegisterViewModel(
            TextEditorFacts.Json.JsonArrayAsTopLevelTextEditorKey,
            JsonTextEditorViewModelKeyTwo);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyOne);
        
        TextEditorService.AddViewModelToGroup(
            JsonTextEditorGroupKey,
            JsonTextEditorViewModelKeyTwo);
        
        base.OnInitialized();
    }
}