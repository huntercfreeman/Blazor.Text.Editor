using System.Collections.Immutable;
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

    private static readonly TextEditorKey C_SHARP_SOURCE_CODE_TEXT_EDITOR_KEY = 
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
                // JavaScript source code with initial render Syntax Highlighting
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

    private const string SampleJavaScriptSourceCode = @"window.blazorTextEditor = {
    measureFontWidthAndElementHeightByElementId: function (elementId, amountOfCharactersRendered) {
        let element = document.getElementById(elementId);
        
        let fontWidth = element.offsetWidth / amountOfCharactersRendered;
        
        return {
            FontWidthInPixels: fontWidth,
            ElementHeightInPixels: element.offsetHeight
        }
    },
    measureWidthAndHeightByElementId: function (elementId) {
        let element = document.getElementById(elementId);
        
        return {
            WidthInPixels: element.offsetWidth,
            HeightInPixels: element.offsetHeight
        }
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let element = document.getElementById(elementId);
        
        let bounds = element.getBoundingClientRect();
        
        let x = clientX - bounds.left;
        let y = clientY - bounds.top;
        
        return {
            RelativeX: x,
            RelativeY: y,
            RelativeScrollLeft: element.scrollLeft,
            RelativeScrollTop: element.scrollTop
        }
    },
    intersectionObserverMap: new Map(),
    initializeIntersectionObserver: function (intersectionObserverMapKey,
                                              virtualizationDisplayDotNetObjectReference,
                                              scrollableParentFinder,
                                              boundaryIds) {

        let scrollableParent = scrollableParentFinder.parentElement;

        scrollableParent.addEventListener(""scroll"", (event) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.intersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples.length; i++) {
                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples[i];

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync(""OnScrollEventAsync"", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, true);

        let options = {
            root: scrollableParent,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.intersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples
                    .find(x => x.BoundaryId === entry.target.id);

                boundaryTuple.IsIntersecting = entry.isIntersecting;

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync(""OnScrollEventAsync"", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, options);

        let boundaryIdIntersectionRatioTuples = [];

        for (let i = 0; i < boundaryIds.length; i++) {

            let boundaryElement = document.getElementById(boundaryIds[i]);

            intersectionObserver.observe(boundaryElement);

            boundaryIdIntersectionRatioTuples.push({
                BoundaryId: boundaryIds[i],
                IsIntersecting: false
            });
        }

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            BoundaryIdIntersectionRatioTuples: boundaryIdIntersectionRatioTuples
        });

        virtualizationDisplayDotNetObjectReference
            .invokeMethodAsync(""OnScrollEventAsync"", {
                ScrollLeftInPixels: scrollableParent.scrollLeft,
                ScrollTopInPixels: scrollableParent.scrollTop
            });
    },
    disposeIntersectionObserver: function (intersectionObserverMapKey) {

        // TODO: Wrong

        let intersectionObserver = this.intersectionObserverMap.get(intersectionObserverMapKey);

        this.intersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserver.disconnect();
    },
    readClipboard: async function () {
        // First, ask the Permissions API if we have some kind of access to
        // the ""clipboard-read"" feature.

        try {
            return await navigator.permissions.query({ name: ""clipboard-read"" }).then(async (result) => {
                // If permission to read the clipboard is granted or if the user will
                // be prompted to allow it, we proceed.

                if (result.state === ""granted"" || result.state === ""prompt"") {
                    return await navigator.clipboard.readText().then((data) => {
                        return data;
                    });
                }
                else {
                    return """";
                }
            });
        }
        catch (e) {
            return """";
        }
    },
    setClipboard: function (value) {
        // Copies a string to the clipboard. Must be called from within an
        // event handler such as click. May return false if it failed, but
        // this is not always possible. Browser support for Chrome 43+,
        // Firefox 42+, Safari 10+, Edge and Internet Explorer 10+.
        // Internet Explorer: The clipboard feature may be disabled by
        // an administrator. By default a prompt is shown the first
        // time the clipboard is used (per session).
        if (window.clipboardData && window.clipboardData.setData) {
            // Internet Explorer-specific code path to prevent textarea being shown while dialog is visible.
            return window.clipboardData.setData(""Text"", text);

        }
        else if (document.queryCommandSupported && document.queryCommandSupported(""copy"")) {
            var textarea = document.createElement(""textarea"");
            textarea.textContent = value;
            textarea.style.position = ""fixed"";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            textarea.select();
            try {
                return document.execCommand(""copy"");  // Security exception may be thrown by some browsers.
            }
            catch (ex) {
                console.warn(""Copy to clipboard failed."", ex);
                return false;
            }
            finally {
                document.body.removeChild(textarea);
            }
        }
    }
}
";
}