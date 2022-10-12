using System.Collections.Immutable;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using ReplApp.SyntaxHighlighting.FictitiousLanguage;
using FictitiousLanguage.ClassLib;
using FictitiousLanguage.ClassLib.Classes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxNodes;
using FictitiousLanguage.ClassLib.Interfaces.SyntaxTokens;
using Microsoft.AspNetCore.Components;

namespace ReplApp.Pages;

public partial class Repl : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private const int MINIMUM_TEXT_EDITOR_WIDTH_IN_PIXELS = 100;
    private const int MINIMUM_TEXT_EDITOR_HEIGHT_IN_PIXELS = 100;
    
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
    
    private bool _shouldRemeasureFlag;
    private int _textEditorWidthInPixels = 400;
    private int _textEditorHeightInPixels = 400;

    private int TextEditorWidthInPixels
    {
        get => _textEditorWidthInPixels;
        set
        {
            if (value > MINIMUM_TEXT_EDITOR_WIDTH_IN_PIXELS)
                _textEditorWidthInPixels = value;
            else
                _textEditorWidthInPixels = MINIMUM_TEXT_EDITOR_WIDTH_IN_PIXELS;

            _shouldRemeasureFlag = !_shouldRemeasureFlag;
        }
    }
    private int TextEditorHeightInPixels
    {
        get => _textEditorHeightInPixels;
        set
        {
            if (value > MINIMUM_TEXT_EDITOR_HEIGHT_IN_PIXELS)
                _textEditorHeightInPixels = value;
            else
                _textEditorHeightInPixels = MINIMUM_TEXT_EDITOR_HEIGHT_IN_PIXELS;

            _shouldRemeasureFlag = !_shouldRemeasureFlag;
        }
    }
    
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

            IncrementHistoryIndex();
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

    private void DecrementHistoryIndex()
    {
        if (_runCodeHistoryIndex > 0)
            _runCodeHistoryIndex--;
    }
    
    private void IncrementHistoryIndex()
    {
        if (_runCodeHistoryIndex is null &&
            _runCodeHistoryList.Any())
        {
            _runCodeHistoryIndex = 0;
        }
        else if (_runCodeHistoryIndex < _runCodeHistoryList.Count - 1)
        {
            _runCodeHistoryIndex++;
        }
    }
}