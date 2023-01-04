# Blazor.Text.Editor

## Usage

### Goal

- Modify Pages/Index.razor to render a C# Text Editor with Syntax Highlighting

### Steps
- One can write the upcoming code however they would like, I will be using a `codebehind`.

- Add in `Pages/` a file named `Index.razor.cs`. Ensure the templated class name is `Index`. Mark this class as `partial`, and inherit from `ComponentBase`

- Within the `Index codebehind` proceed to inject the `ITextEditorService`.

- At this stage my Index.razor.cs looks like the following:

```csharp
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditorDocumentation.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
}
```

- In order to change pages and `maintain` the TextEditor's `state` the `TextEditorKey` which is used `must not change`.

- One can maintain a reference to an unchanging `TextEditorKey` in various ways. I will be making a static readonly `TextEditorKey` within the `Index codebehind`.

- To make a `TextEditorKey` one should use the factory method: `TextEditorKey.NewTextEditorKey()`

```csharp
private static readonly TextEditorKey IndexTextEditorKey = TextEditorKey.NewTextEditorKey();
```

- Now that we have a `TextEditorKey` we can override the Blazor lifecycle method, `OnInitialized`.

- In `OnInitialized` note the autocomplete options when one is to type `TextEditorService.Register`.

- `RegisterRazorTextEditor` is available, `RegisterCSharpTextEditor` is available, and so on. 

- These methods ease the creation of specific Text Editors like one for C#. Otherwise one has to provide the corresponding ILexer and IDecorationMapper themselves.

- Should one wish to pass in the ILexer and IDecorationMapper themselves then use `RegisterCustomTextEditor`.

- In this step we specifically will be invoking `TextEditorService.RegisterCSharpTextEditor(...);` to get a C# Text Editor with Syntax Highlighting.

- The `RegisterCSharpTextEditor` method wants us to provide as arguments:
    - `TextEditorKey textEditorKey`
    - `string resourceUri`
    - `DateTime resourceLastWriteTime`
    - `string fileExtension`
    - `string initialContent`

- I will invoke the method as shown in the following code snippet

```csharp
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditorDocumentation.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorKey IndexTextEditorKey = TextEditorKey.NewTextEditorKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditor(
            IndexTextEditorKey,
            "/users/individual/home/ExampleCSharp.cs",
            DateTime.Now,
            "C#",
            string.Empty);
        
        base.OnInitialized();
    }
}
```

- Add the following using statement to Index.razor if it is not already there.

```html
@using BlazorTextEditor.RazorLib
```

- Render to Index.razor a self closing Blazor component by the name of `TextEditorViewModelDisplay`. See the following code snippet

```html
<TextEditorViewModelDisplay/>
```

- The `TextEditorViewModelDisplay` Blazor component has many Blazor parameters available for customization.

- However, only one parameter is needed to render a `TextEditorViewModelDisplay` and that is `TextEditorViewModelKey`.

- We can return to the `Index codebehind` to create a `TextEditorViewModel.cs`. The step prior to this registered a `TextEditorBase.cs`.

- It is important to note that `TextEditorBase.cs` maps to a unique resource, perhaps a unique file on one's file system. Whereas `TextEditorViewModel.cs` maps to the user interface state of a Blazor component. Many `TextEditorViewModel.cs` can reference the same `TextEditorBase.cs`.

- I will be making a static readonly `TextEditorViewModelKey` within the `Index codebehind`. Having `TextEditorViewModelKey` be static and readonly allows the user interface to maintain its state when navigating pages.

- To make a `TextEditorViewModelKey` one should use the factory method: `TextEditorViewModelKey.NewTextEditorViewModelKey()`

```csharp
private static readonly TextEditorViewModelKey IndexTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
```

- Now that we have a `TextEditorViewModelKey` in the Blazor lifecycle method `OnInitialized` we can register a `TextEditorViewModel.cs`.

- To register a `TextEditorViewModel.cs` invoke the method: `TextEditorService.RegisterViewModel(...)` which has arguments:
    - TextEditorViewModelKey textEditorViewModelKey
    - TextEditorKey textEditorKey

- Pass in the `IndexTextEditorViewModelKey` and the `IndexTextEditorKey`.

- My `Index codebehind` is overriding `OnInitialized` as the following:

```csharp
protected override void OnInitialized()
{
    TextEditorService.RegisterCSharpTextEditor(
        IndexTextEditorKey,
        "/users/individual/home/ExampleCSharp.cs",
        DateTime.Now,
        "C#",
        string.Empty);
    
    TextEditorService.RegisterViewModel(
        IndexTextEditorViewModelKey,
        IndexTextEditorKey);
    
    base.OnInitialized();
}
```

- Pass to the `TextEditorViewModelDisplay` the Blazor parameter named, `TextEditorViewModelKey` with the value of `IndexTextEditorViewModelKey`. See the following code snippet.

```html
<TextEditorViewModelDisplay TextEditorViewModelKey="IndexTextEditorViewModelKey"/>
```

- Now run the application and you will have rendered a `TextEditorViewModelDisplay` with C# syntax highlighting.

![Rendered C# Text Editor](/Images/Gifs/10_usage-rendered.gif)

# Next tutorial: [Settings](/Documentation/20_SETTINGS.md)