using System.Collections.Immutable;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.TextEditor;
using ExampleApplication.SyntaxHighlighting.CSharp;
using ExampleApplication.SyntaxHighlighting.FictitiousLanguage;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static readonly TextEditorKey C_SHARP_SOURCE_CODE_TEXT_EDITOR_KEY = 
        TextEditorKey.NewTextEditorKey();
    
    private static readonly TextEditorKey FICTITIOUS_LANGUAGE_SOURCE_CODE_TEXT_EDITOR_KEY = 
        TextEditorKey.NewTextEditorKey();
    
    private static readonly TextEditorKey MARY_HAD_A_LITTLE_LAMB_TEXT_EDITOR_KEY = 
        TextEditorKey.NewTextEditorKey();
    
    private static readonly ImmutableArray<Func<Task<TextEditorBase>>> INITIAL_TEXT_EDITOR_CONSTRUCTS =
            new Func<Task<TextEditorBase>>[]
            {
                // C# source code with initial render Syntax Highlighting
                async () =>
                {
                    var textEditorBase = new TextEditorBase(
                        SampleCSharpSourceCode,
                        new TextEditorCSharpLexer(),
                        new TextEditorCSharpDecorationMapper(),
                        C_SHARP_SOURCE_CODE_TEXT_EDITOR_KEY);

                    await textEditorBase.ApplySyntaxHighlightingAsync();

                    return textEditorBase;
                },
                // FictitiousLanguage source code with initial render Syntax Highlighting
                async () =>
                {
                    var textEditorBase = new TextEditorBase(
                        SampleFictitiousLanguageSourceCode,
                        new TextEditorFictitiousLanguageLexer(),
                        new TextEditorFictitiousLanguageDecorationMapper(),
                        FICTITIOUS_LANGUAGE_SOURCE_CODE_TEXT_EDITOR_KEY);

                    await textEditorBase.ApplySyntaxHighlightingAsync();

                    return textEditorBase;
                },
                // Mary had a little lamb: https://www.poetryfoundation.org/poems/46954/mary-had-a-little-lamb
                () =>
                {
                    var textEditorBase = new TextEditorBase(
                        MaryHadALittleLamb,
                        null,
                        null,
                        MARY_HAD_A_LITTLE_LAMB_TEXT_EDITOR_KEY);

                    return Task.FromResult(textEditorBase);
                }
            }.ToImmutableArray();

    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;

        base.OnInitialized();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // NOTE: I am hackily putting test TextEditors on the
            // screen like this for debugging.
            
            foreach (var initialTextEditorConstruct in INITIAL_TEXT_EDITOR_CONSTRUCTS)
            {
                var textEditor = await initialTextEditorConstruct.Invoke();
                
                TextEditorService.RegisterTextEditor(textEditor);
            }
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

    private const string SampleCSharpSourceCode = @"using System.Collections.Immutable;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.TextEditor;
using ExampleApplication.SyntaxHighlighting.CSharp;
using Microsoft.AspNetCore.Components;

namespace ExampleApplication.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private static bool _hasInitialized;
    
    private static readonly ImmutableArray<Func<Task<TextEditorBase>>> INITIAL_TEXT_EDITOR_CONSTRUCTS =
            new Func<Task<TextEditorBase>>[]
            {
                // C# source code with initial render Syntax Highlighting
                async () =>
                {
                    var textEditorBase = new TextEditorBase(
                        SampleCSharpSourceCode,
                        new TextEditorCSharpLexer(),
                        new TextEditorCSharpDecorationMapper());

                    await textEditorBase.ApplySyntaxHighlightingAsync();

                    return textEditorBase;
                }
            }.ToImmutableArray();

    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;

        base.OnInitialized();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!_hasInitialized)
            {
                _hasInitialized = true;
                
                foreach (var initialTextEditorConstruct in INITIAL_TEXT_EDITOR_CONSTRUCTS)
                {
                    var textEditor = await initialTextEditorConstruct.Invoke();
                    
                    TextEditorService.RegisterTextEditor(textEditor);
                }
            }
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
    }";

    private const string SampleFictitiousLanguageSourceCode = "var x; x = 2;";
    
    private const string MaryHadALittleLamb = @"""Mary had a little lamb,""
BY SARAH JOSEPHA HALE
Mary had a little lamb,
Its fleece was white as snow;
And everywhere that Mary went
The lamb was sure to go.

It followed her to school one day,
Which was against the rule;
It made the children laugh and play
To see a lamb at school.

And so the teacher turned it out,
But still it lingered near,
And waited patiently about
Till Mary did appear.

Why does the lamb love Mary so?
The eager children cry;
Why, Mary loves the lamb, you know,
The teacher did reply.";
}