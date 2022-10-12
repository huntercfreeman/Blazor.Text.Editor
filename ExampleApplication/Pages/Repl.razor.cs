using System.Collections.Immutable;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using ExampleApplication.SyntaxHighlighting.CSharp;
using ExampleApplication.SyntaxHighlighting.FictitiousLanguage;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.Pages;

public partial class Repl : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorKey REPL_TEXT_EDITOR_KEY = 
        TextEditorKey.NewTextEditorKey();
    
    private TextEditorBase? ReplTextEditor => TextEditorService
        .TextEditorStates
        .TextEditorList
        .SingleOrDefault(x => x.Key == REPL_TEXT_EDITOR_KEY);
    
    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;

        base.OnInitialized();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var textEditor = new TextEditorBase(
                string.Empty,
                new TextEditorFictitiousLanguageLexer(),
                new TextEditorFictitiousLanguageDecorationMapper(),
                REPL_TEXT_EDITOR_KEY);
            
            TextEditorService.RegisterTextEditor(textEditor);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async void TextEditorServiceOnOnTextEditorStatesChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void RegisterTextEditorOnClick()
    {
        TextEditorService.RegisterTextEditor(
            new TextEditorBase(
                string.Empty,
                null,
                null));
    }
    
    public void Dispose()
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    }
}