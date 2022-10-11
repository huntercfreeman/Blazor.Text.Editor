# Blazor.Text.Editor - documentation

## Installation and Setup

### Goal
- This tutorial will use the default Blazor ServerSide template.
- A button is put on the Index.razor page. When this button is clicked a text editor instance will be registered by invoking [TextEditorService.RegisterTextEditor](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorService.cs)
- By means of subscribing to the `EventHandler` [TextEditorService.OnTextEditorStatesChanged](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorService.cs) the Index.razor will be notified to re-render whenever a text editor instance is either registered or disposed.
- There is a foreach() loop in the Index.razor markup that renders a [TextEditorDisplay](https://github.com/huntercfreeman/BlazorTextEditorNugetPackage/blob/dev-version-1.1.0/BlazorTextEditor.RazorLib/TextEditorDisplay.razor.cs) foreach of the registered text editor instances.

### Steps
- Add a NuGet package reference to `Blazor.Text.Editor`

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

