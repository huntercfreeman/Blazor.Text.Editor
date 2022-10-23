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
    - COLOR THEME :root [blazorTextEditorDefaultColors.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditorDefaultColors.css): `<link href="_content/Blazor.Text.Editor/blazorTextEditorDefaultColors.css" rel="stylesheet"/>`
    - COLOR THEME .bte_dark-theme-visual-studio [blazorTextEditorVisualStudioDarkTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorVisualStudioDarkTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioDarkTheme.css" rel="stylesheet"/>`
    - COLOR THEME .bte_light-theme-visual-studio [blazorTextEditorVisualStudioLightTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorVisualStudioLightTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioDarkTheme.css" rel="stylesheet"/>`
    - COLOR THEME .bte_dark-theme [blazorTextEditorDarkTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorDarkTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorDarkTheme.css" rel="stylesheet"/>`
    - COLOR THEME .bte_light-theme [blazorTextEditorLightTheme.css](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/Themes/blazorTextEditorLightTheme.css): `<link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorLightTheme.css" rel="stylesheet"/>`

- Add the library's JavaScript to `Pages/_Layout.cshtml`. Every available JavaScript file appears in the following child bulleted list. The entirety of an example _Layout.cshtml is shown as an html code snippet after this step.
    - REQUIRED [blazorTextEditor.js](https://github.com/huntercfreeman/Blazor.Text.Editor/blob/main/BlazorTextEditor.RazorLib/wwwroot/blazorTextEditor.js): `<script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>`

- The following html code snippet is an example _Layout.cshtml

```html
@using Microsoft.AspNetCore.Components.Web
@namespace BlazorApp1.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="en">
<head>
    <!-- ... left out unrelated markup for conciseness -->

    <!-- Required .css -->
    <link href="_content/Blazor.Text.Editor/blazorTextEditor.css" rel="stylesheet"/>

    <!-- Optional theme css variables for :root -->
    <link href="_content/Blazor.Text.Editor/blazorTextEditorDefaultColors.css" rel="stylesheet"/>
    
    <!-- ... left out unrelated markup for conciseness -->
</head>
<body>

<!-- ... left out unrelated markup for conciseness -->

<script src="_framework/blazor.server.js"></script>

<script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>

</body>
</html>
```

- Register the library's services by invoking the IServiceCollection extension method `AddTextEditorRazorLibServices()`

```csharp
// In Program.cs

using BlazorTextEditor.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// ... using elipses for conciseness

builder.Services.AddTextEditorRazorLibServices();

var app = builder.Build();
```

- Edit `App.razor` and add the following markup. 

> **_NOTE:_**  Internally the library uses a state management library named [Fluxor](https://github.com/mrpmorris/Fluxor). This markup currently only acts as a wrapper for the markup required by Fluxor which is their [StoreInitializer.razor](https://github.com/mrpmorris/Fluxor/blob/master/Source/Lib/Fluxor.Blazor.Web/StoreInitializer.cs).

```html
<BlazorTextEditor.RazorLib.BlazorTextEditorInitializer />
```

- Add a file named `Index.razor.cs` to the directory `Pages/`

- In `Index.razor.cs` inject the dependency `ITextEditorService` (see the following code snippet)

```csharp
[Inject]
private ITextEditorService TextEditorService { get; set; } = null!;
```

- Subscribe to the event handler `TextEditorService.OnTextEditorStatesChanged` with a method that calls `InvokeAsync(StateHasChanged)`. (see the following code snippet and child bullet points).
    - Implement `IDisposable`
    - In the `Dispose()` method use `-=` to unsubscribe. This avoids a memory leak.
    - In the Blazor lifecycle method `OnInitialized()` use `+=` to subscribe.
    - An example of a method one can subscribe with is `void OnTextEditorStatesChanged(object? sender, EventArgs e)`

```csharp
// In Index.razor.cs

// ... the other using statements
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;

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


