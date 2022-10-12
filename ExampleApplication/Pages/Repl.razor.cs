using System.Collections.Immutable;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using ExampleApplication.SyntaxHighlighting.CSharp;
using ExampleApplication.SyntaxHighlighting.FictitiousLanguage;
using FictitiousLanguage.ClassLib;
using FictitiousLanguage.ClassLib.Classes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.Pages;

public partial class Repl : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorKey REPL_TEXT_EDITOR_KEY = 
        TextEditorKey.NewTextEditorKey();

    private TextEditorDisplay? _textEditorDisplay;

    private TextEditorBase? ReplTextEditor => TextEditorService
        .TextEditorStates
        .TextEditorList
        .SingleOrDefault(x => x.Key == REPL_TEXT_EDITOR_KEY);

    private List<(
        string input, 
        ImmutableArray<ISyntaxToken> syntaxTokens,
        ISyntaxNode? rootSyntaxNode, 
        EvaluatorResult? evaluatorResult,
        ImmutableArray<DiagnosticBlazorStudio> diagnostics)> _runCodeHistoryList = new();

    private int? _runCodeHistoryIndex;
    
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

    private void RunCodeOnClick()
    {
        void AddToReplHistory((
            string input, 
            ImmutableArray<ISyntaxToken> syntaxTokens,
            ISyntaxNode? rootSyntaxNode, 
            EvaluatorResult? evaluatorResult,
            ImmutableArray<DiagnosticBlazorStudio> diagnostics) historyEntry)
        {
            _runCodeHistoryList.Add(historyEntry);

            if (_runCodeHistoryIndex is null)
                _runCodeHistoryIndex = 0;
            else
                _runCodeHistoryIndex++;
        }
        
        void ReportReplUsageError(string message, DiagnosticLevel diagnosticLevel)
        {
            AddToReplHistory((
                    "null",
                    ImmutableArray<ISyntaxToken>.Empty, 
                    null,
                    null,
                    new[]
                    {
                        new DiagnosticBlazorStudio(message, diagnosticLevel)
                    }.ToImmutableArray()
                ));
        }

        if (_textEditorDisplay is null)
        {
            ReportReplUsageError(
                "_textEditorDisplay was null", 
                DiagnosticLevel.Error);

            return;
        }
        
        var localReplTextEditor = ReplTextEditor;

        if (localReplTextEditor is null)
        {
            ReportReplUsageError(
                "localReplTextEditor was null", 
                DiagnosticLevel.Error);
            
            return;
        }
        
        var code = _textEditorDisplay.PrimaryCursor.GetSelectedText(localReplTextEditor);

        if (code is null)
        {
            ReportReplUsageError(
                "select the text you want to run", 
                DiagnosticLevel.Error);
            
            return;
        } 
        
        Lexer lexer = new();
        var syntaxTokens = lexer.Lex(code);
        
        Parser parser = new();
        var rootSyntaxNode = parser.Parse(syntaxTokens);

        Evaluator evaluator = new();
        var evaluatorResult = evaluator.Evaluate(rootSyntaxNode);

        var allDiagnostics = lexer.Diagnostics
            .Union(parser.Diagnostics)
            .Union(evaluator.Diagnostics)
            .ToImmutableArray();
        
        AddToReplHistory((code, syntaxTokens, rootSyntaxNode, evaluatorResult, allDiagnostics));
    }

    private string MapDiagnosticLevelToCssClass(DiagnosticLevel diagnosticLevel)
    {
        return diagnosticLevel switch
        {
            DiagnosticLevel.Info => "ea_diagnostic-level-info",
            DiagnosticLevel.Warning => "ea_diagnostic-level-warning",
            DiagnosticLevel.Error => "ea_diagnostic-level-error",
            _ => throw new ApplicationException($"The {nameof(DiagnosticLevel)} {diagnosticLevel} was unrecognized.")
        };
    }
    
    public void Dispose()
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    }
}