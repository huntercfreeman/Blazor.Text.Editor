# Blazor.Text.Editor - documentation

## Installation and Setup

### Goal
- This tutorial will use the default Blazor ServerSide template.
- A button is put on the Index.razor page. When this button is clicked a text editor instance will be registered by invoking [TextEditorService.RegisterTextEditor](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorService.cs)
- By means of subscribing to the `EventHandler` [TextEditorService.OnTextEditorStatesChanged](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorService.cs) the Index.razor will be notified to re-render whenever a text editor instance is either registered or disposed.
- There is a foreach() loop in the Index.razor markup that renders a [TextEditorDisplay](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorDisplay.razor.cs) foreach of the registered text editor instances.

### Steps
- Add a NuGet package reference to `Blazor.Text.Editor`

- Add the library's css stylesheets to `Pages/_Layout.cshtml`. Every available stylesheet appears in the following child bulleted list. The entirety of an example _Layout.cshtml is shown as an html code snippet after the next step.
    - REQUIRED [blazorTextEditor.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditor.css): `<link href="_content/Blazor.Text.Editor/blazorTextEditor.css" rel="stylesheet"/>`
    - REQUIRED [blazorTextEditorSizes.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditorSizes.css): `<link href="_content/Blazor.Text.Editor/blazorTextEditorSizes.css" rel="stylesheet"/>`
    - COLOR THEME :root [blazorTextEditorDefaultColors.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditorDefaultColors.css): `<link href="_content/Blazor.Text.Editor/blazorTextEditorDefaultColors.css" rel="stylesheet"/>`
    - COLOR THEME .bte_dark-theme-visual-studio [blazorTextEditorVisualStudioDarkTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorVisualStudioDarkTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioDarkTheme.css" rel="stylesheet"/>`
    - COLOR THEME .bte_light-theme-visual-studio [blazorTextEditorVisualStudioLightTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorVisualStudioLightTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioLightTheme.css" rel="stylesheet"/>`

- Add the library's JavaScript to `Pages/_Layout.cshtml`. Every available JavaScript file appears in the following child bulleted list. The entirety of an example _Layout.cshtml is shown as an html code snippet after this step.
    - REQUIRED [blazorTextEditor.js](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditor.js): `<script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>`

- The following html code snippet is an example index.html with unrelated things ommitted for brevity.

```html

<head>
    @* Required CSS *@
    <link href="_content/Blazor.Text.Editor/blazorTextEditor.css" rel="stylesheet"/>
    @* Required CSS *@
    <link href="_content/Blazor.Text.Editor/blazorTextEditorSizes.css" rel="stylesheet"/>
    
    @* Default Theme (is same as Visual Studio Dark Clone) | :root {...} *@
    <link href="_content/Blazor.Text.Editor/blazorTextEditorDefaultColors.css" rel="stylesheet"/>
    
    @* Visual Studio Dark Clone Theme *@
    <link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioDarkTheme.css" rel="stylesheet"/>
    @* Visual Studio Light Clone Theme *@
    <link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioLightTheme.css" rel="stylesheet"/>
    
</head>
<body>

    <script src="_framework/blazor.server.js"></script>

    @* Required JavaScript *@
    <script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>

</body>
</html>

```

- Register the library's services by invoking the IServiceCollection extension method `AddBlazorTextEditor()`

```csharp
// In Program.cs

using BlazorTextEditor.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// ... using elipses for conciseness

builder.Services.AddBlazorTextEditor();

var app = builder.Build();
```

- Edit `App.razor` and add the following markup. 

```html
<BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/>
```

- Add a file named `Index.razor.cs` to the directory `Pages/`

- In `Index.razor.cs` inject the dependency `ITextEditorService` (see the following code snippet)

```csharp
[Inject]
private ITextEditorService TextEditorService { get; set; } = null!;
```

- Subscribe to the event Action `TextEditorService.OnTextEditorStatesChanged` with a method that calls `InvokeAsync(StateHasChanged)`. (see the following code snippet and child bullet points).
    - Implement `IDisposable`
    - In the `Dispose()` method use `-=` to unsubscribe. This avoids a memory leak.
    - In the Blazor lifecycle method `OnInitialized()` use `+=` to subscribe.
    - An example of a method one can subscribe with is `void TextEditorServiceOnOnTextEditorStatesChanged()`

```csharp
// In Index.razor.cs

using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditorDemo.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;
        
        base.OnInitialized();
    }

    private void TextEditorServiceOnOnTextEditorStatesChanged()
    {
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnOnTextEditorStatesChanged;
    }
}
```

- Create a method named `RegisterTextEditorOnClick` that returns `void`.

- Implement `RegisterTextEditorOnClick` such that it invokes the method `TextEditorService.RegisterTextEditor()`

- `TextEditorService.RegisterTextEditor()` takes as a parameter an instance of [TextEditorBase](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditor/TextEditorBase.cs). The most basic construction of a `TextEditorBase` is showcased in the following blockquote.

> new [TextEditorBase](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditor/TextEditorBase.cs)(string.Empty, default([ILexer](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/Lexing/ILexer.cs)), default([IDecorationMapper](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/Decoration/IDecorationMapper.cs)))

- The following code snippet is the entirety of an example Index.razor.cs

```csharp
// Full example of an Index.razor.cs

using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;
        
        base.OnInitialized();
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
```

- Next we need to modify Index.razor

- Add a button to Index.razor that invokes the method `RegisterTextEditorOnClick` which is located in Index.razor.cs (see the following markup)

```html
<button class="btn btn-primary"
        @onclick="RegisterTextEditorOnClick">
    RegisterTextEditorOnClick
</button>
```

- Add a using for `BlazorTextEditor.RazorLib` at the top of Index.razor (see the following markup)

```html
@using BlazorTextEditor.RazorLib
```

- At the bottom of Index.razor.cs add an unimplemented foreach loop that iterates over `TextEditorService.TextEditorStates.TextEditorList` (see the following markup)

```html
@foreach (var textEditor in TextEditorService.TextEditorStates.TextEditorList)
{
    <!-- No Implementation Yet -->
}
```



- For the innards of the foreach loop render out a [TextEditorDisplay.razor.cs](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorDisplay.razor.cs) foreach of the text editor instances (see the following markup)

```html
@foreach (var textEditor in TextEditorService.TextEditorStates.TextEditorList)
{
    <!-- @key is only used due to this being done within a foreach loop -->
    <TextEditorDisplay @key="textEditor.Key" 
                       TextEditorKey="textEditor.Key"
                       StyleCssString="width: 400px; height: 400px;" />
}
```

> **_NOTE:_** [TextEditorDisplay.razor.cs](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorDisplay.razor.cs) has a Blazor parameter for applying css classes named `ClassCssString`



### Syntax Highlighting / ILexer.cs
- Add a NuGet package reference to:
    - `Blazor.Text.Editor.Lexer.CSharp` [(see on nuget.org)](https://www.nuget.org/packages/Blazor.Text.Editor.Lexer.CSharp/)
    - `Blazor.Text.Editor.Lexer.HTML` [(see on nuget.org)](https://www.nuget.org/packages/Blazor.Text.Editor.Lexer.HTML)

- In Index.razor.cs
- Add two TextEditorKeys

```csharp
private static readonly TextEditorKey _cSharpTextEditorKey = TextEditorKey.NewTextEditorKey();
private static readonly TextEditorKey _htmlTextEditorKey = TextEditorKey.NewTextEditorKey();
```

In override OnInitialized() register two text editors using the two keys and pass in the corresponding ILexer and IDecorationMapper

```csharp
protected override void OnInitialized()
{
    TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnOnTextEditorStatesChanged;
    
    TextEditorService.RegisterTextEditor(
        new TextEditorBase(
            string.Empty,
            new TextEditorCSharpLexer(),
            new TextEditorCSharpDecorationMapper(),
            _cSharpTextEditorKey));
    
    TextEditorService.RegisterTextEditor(
        new TextEditorBase(
            string.Empty,
            new TextEditorHtmlLexer(),
            new TextEditorHtmlDecorationMapper(),
            _htmlTextEditorKey));
    
    base.OnInitialized();
}
```

Add the following method to apply syntax highlighting as the user types

```csharp
private async Task OnAfterOnKeyDownAsync(
        TextEditorBase textEditor, 
        ImmutableTextEditorCursor immutableTextEditorCursor, 
        KeyboardEventArgs keyboardEventArgs, 
        Func<TextEditorMenuKind, Task> displayMenuFunc)
    {
        if (keyboardEventArgs.Key == ";" ||
            KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code))
        {
            await textEditor.ApplySyntaxHighlightingAsync();
        }

        await InvokeAsync(StateHasChanged);
    }
```

In Index.razor add the following markup

```html
@{
    var cSharpTextEditor = TextEditorService.TextEditorStates.TextEditorList
        .FirstOrDefault(x => x.Key == _cSharpTextEditorKey);

    if (cSharpTextEditor is not null)
    {
        <h3>C# Text Editor</h3>
         <TextEditorDisplay @ref="_cSharpTextEditorDisplay"
                            TextEditorKey="_cSharpTextEditorKey"
                            StyleCssString="width: 400px; height: 400px;"
                            AfterOnKeyDownAsync="OnAfterOnKeyDownAsync"/>
    }
}

@{
    var htmlTextEditor = TextEditorService.TextEditorStates.TextEditorList
        .FirstOrDefault(x => x.Key == _htmlTextEditorKey);

    if (htmlTextEditor is not null)
    {
        <h3>HTML Text Editor</h3>
         <TextEditorDisplay @ref="_htmlTextEditorDisplay"
                            TextEditorKey="_htmlTextEditorKey"
                            StyleCssString="width: 400px; height: 400px;"
                            AfterOnKeyDownAsync="OnAfterOnKeyDownAsync" />
    }
}
```



