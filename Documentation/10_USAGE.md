# Blazor.Text.Editor
*NOTE*: Last Modified: (v7.0.0)

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

- In order to change pages and `maintain` the TextEditor's `state` the `TextEditorModelKey` which is used `must not change`.

- One can maintain a reference to an unchanging `TextEditorModelKey` in various ways. I will be making a static readonly `TextEditorModelKey` within the `Index codebehind`.

- To make a `TextEditorModelKey` one should use the factory method: `TextEditorModelKey.NewTextEditorModelKey()`

```csharp
private static readonly TextEditorModelKey IndexTextEditorModelKey = TextEditorModelKey.NewTextEditorModelKey();
```

- Now that we have a `TextEditorModelKey` we can override the Blazor lifecycle method, `OnInitialized`.

- In `OnInitialized` note the autocomplete options when one is to type `TextEditorService.Register`. This is shown in the following GIF:

![Render the Initializer component](/Images/Gifs/v7.0.0/TextEditorService.Register.gif)

- `ModelRegisterTemplatedModel` is the method we are looking for.

- `TextEditorService.ModelRegisterTemplatedModel(...)` eases the creation of specific Text Editors like one for C#. Otherwise one has to provide the corresponding ILexer and IDecorationMapper themselves.

- Should one wish to pass in the ILexer and IDecorationMapper themselves then use `TextEditorService.ModelRegisterCustomModel(...)`.

- Let's invoke `TextEditorService.ModelRegisterTemplatedModel(...);` as to get a C# Text Editor with Syntax Highlighting.

- The `ModelRegisterTemplatedModel` method wants us to provide as arguments:
    - `TextEditorModelKey` textEditorModelKey
    - `WellKnownModelKind` wellKnownModelKind
    - `string` resourceUri
    - `DateTime` resourceLastWriteTime
    - `string` fileExtension
    - `string` initialContent

- I will invoke the method as shown in the following code snippet. As well the previous gif contained me writing this code snippet

```csharp
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using Microsoft.AspNetCore.Components;

namespace BlazorClassrooms.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorModelKey IndexTextEditorModelKey = 
        TextEditorModelKey.NewTextEditorModelKey();

    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            IndexTextEditorModelKey,
            WellKnownModelKind.CSharp,
            "placeholder.cs",
            DateTime.UtcNow,
            ".cs",
            string.Empty);
        
        base.OnInitialized();
    }
}
```

- Add the following using statement to Index.razor if it is not already there.

```html
@using BlazorTextEditor.RazorLib.ViewModel
```

- Render to Index.razor a self closing Blazor component by the name of `TextEditorViewModelDisplay`. See the following code snippet

```html
@page "/"

@using BlazorTextEditor.RazorLib.ViewModel

<!-- Omitted Templated Blazor Markup for Conciseness -->

<TextEditorViewModelDisplay/>
```

- The `TextEditorViewModelDisplay` Blazor component has many Blazor parameters available for customization.

- However, only one parameter is needed to render a `TextEditorViewModelDisplay` and that is `TextEditorViewModelKey`.

- We can return to the `Index codebehind` to create a `TextEditorViewModel.cs`. The step prior to this registered a `TextEditorModel.cs`.

- It is important to note that a `TextEditorModel.cs` instance maps to a unique resource, perhaps a unique file on one's file system. Whereas a `TextEditorViewModel.cs` instance maps to the user interface state of a Blazor component.

- Many `TextEditorViewModel.cs` instances can reference the same `TextEditorModel.cs` instance. Then, when the underlying `TextEditorModel.cs` instance is changed, all of the `TextEditorViewModel.cs` instances that reference it will be notified to re-render with the changes.

- I will be making a static readonly `TextEditorViewModelKey` within the `Index codebehind`. Having `TextEditorViewModelKey` be static and readonly allows the user interface to maintain its state when navigating pages.

- To make a `TextEditorViewModelKey` one should use the factory method: `TextEditorViewModelKey.NewTextEditorViewModelKey()`

```csharp
private static readonly TextEditorViewModelKey IndexTextEditorViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
```

- Now that we have a `TextEditorViewModelKey`, in the Blazor lifecycle method `OnInitialized` we can register a `TextEditorViewModel.cs`.

- To register a `TextEditorViewModel.cs` invoke the method: `TextEditorService.ViewModelRegister(...)` which has arguments:
    - `TextEditorViewModelKey` textEditorViewModelKey
    - `TextEditorModelKey` textEditorModelKey

- Pass in the `IndexTextEditorViewModelKey` and the `IndexTextEditorModelKey`.

- My `Index codebehind` is overriding the `OnInitialized` method as the following:

```csharp
protected override void OnInitialized()
{
    TextEditorService.ModelRegisterTemplatedModel(
        IndexTextEditorModelKey,
        WellKnownModelKind.CSharp,
        "placeholder.cs",
        DateTime.UtcNow,
        ".cs",
        string.Empty);
    
    TextEditorService.ViewModelRegister(
        IndexTextEditorViewModelKey,
        IndexTextEditorModelKey);
    
    base.OnInitialized();
}
```

- Pass to the `TextEditorViewModelDisplay` the Blazor parameter named, `TextEditorViewModelKey` with the value of `IndexTextEditorViewModelKey`. See the following code snippet.

```html
<TextEditorViewModelDisplay TextEditorViewModelKey="IndexTextEditorViewModelKey"/>
```

- Now run the application and you will have rendered a `TextEditorViewModelDisplay` with C# syntax highlighting.

- When the user hits "Ctrl + s" to save, let's get the text value of the TextEditorModel.cs instance we registered.

- `TextEditorViewModel.cs` has a property:
    - `Action<TextEditorModel>?` OnSaveRequested

- We can only modify a `TextEditorViewModel.cs` through the `TextEditorService.cs`.

- In the overriden `OnInitialized` method. Add furthermore to what we have an invocation of `TextEditorService.ViewModelWith(...)`.

- `TextEditorService.ViewModelWith(...)` method wants us to provide as arguments:
    - `TextEditorViewModelKey` textEditorViewModelKey
    - `Func<TextEditorViewModel, TextEditorViewModel>` withFunc

- `TextEditorService.ViewModelWith(...)` will perform a synchronous and concurrent replacement of the `TextEditorViewModel.cs` record instance.

- Pass in to `ViewModelWith(...)` the `IndexTextEditorViewModelKey`. As well, pass in a lamda expression which takes in the current `TextEditorViewModel.cs`, and outputs the next instance. Since `TextEditorViewModel.cs` is a `record` datatype we can use the `with` keyword to preserve any data we don't wish to change.

- The following code snippet is my override for the `OnInitialized` method as of this step having been completed.

```csharp
protected override void OnInitialized()
{
    TextEditorService.ModelRegisterTemplatedModel(
        IndexTextEditorModelKey,
        WellKnownModelKind.CSharp,
        "placeholder.cs",
        DateTime.UtcNow,
        ".cs",
        string.Empty);
    
    TextEditorService.ViewModelRegister(
        IndexTextEditorViewModelKey,
        IndexTextEditorModelKey);
    
    TextEditorService.ViewModelWith(
        IndexTextEditorViewModelKey,
        inViewModel => inViewModel with
        {
            OnSaveRequested = HandleOnSaveRequested 
        });
    
    base.OnInitialized();
}
```

- Note that in my `with` keyword usage. I am setting the `OnSaveRequested` Action to a method named `HandleOnSaveRequested`.

- The `OnSaveRequested` Action takes in as a parameter:
    - `TextEditorModel` textEditorModel

- Therefore the method I am assigning it with has the following method signature:
    - private void `HandleOnSaveRequested`(TextEditorModel textEditorModel)

- The following code snippet is the entirety of my `HandleOnSaveRequested` method.

```csharp
private void HandleOnSaveRequested(
        TextEditorModel textEditorModel)
{
    _savedContent = textEditorModel.GetAllText();
    
    // The executing thread might not be the UI thread.
    //
    // Use 'InvokeAsync' to guarantee that Blazor will
    // run 'StateHasChanged' on the UI thread.
    InvokeAsync(StateHasChanged);
}
```

- Inside of the `HandleOnSaveRequested` method one can do whatever they would like to. In my implementation I am setting a private field of type string named `_savedContent` to the result of invoking textEditorModel.`GetAllText`();.

- Likely, instead of storing the user's saved content to a private field one might store it in a database of sorts.

- Before continuing, the following code snippet is the entirety of my Index.razor.cs file as of this point.

```csharp
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Microsoft.AspNetCore.Components;

namespace BlazorClassrooms.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorModelKey IndexTextEditorModelKey = 
        TextEditorModelKey.NewTextEditorModelKey();
    
    private static readonly TextEditorViewModelKey IndexTextEditorViewModelKey = 
        TextEditorViewModelKey.NewTextEditorViewModelKey();

    private string _savedContent = string.Empty;

    protected override void OnInitialized()
    {
        TextEditorService.ModelRegisterTemplatedModel(
            IndexTextEditorModelKey,
            WellKnownModelKind.CSharp,
            "placeholder.cs",
            DateTime.UtcNow,
            ".cs",
            string.Empty);
        
        TextEditorService.ViewModelRegister(
            IndexTextEditorViewModelKey,
            IndexTextEditorModelKey);
        
        TextEditorService.ViewModelWith(
            IndexTextEditorViewModelKey,
            inViewModel => inViewModel with
            {
                OnSaveRequested = HandleOnSaveRequested 
            });
        
        base.OnInitialized();
    }

    private void HandleOnSaveRequested(
        TextEditorModel textEditorModel)
    {
        _savedContent = textEditorModel.GetAllText();
        
        // The executing thread might not be the UI thread.
        //
        // Use 'InvokeAsync' to guarantee that Blazor will
        // run 'StateHasChanged' on the UI thread.
        InvokeAsync(StateHasChanged);
    }
}
```

- I would like to display the saved content on the user interface just to make clear what invoking `textEditorModel.GetAllText();` returns.

- In order to preserve whitespace I will use a &lt;`pre`&gt; tag. But beware, I believe similarly to a MarkupString. If the string content displayed in a &lt;`pre`&gt; tag is from an unknown source. You might be vulnerable to a script injection attack.

- Here is the entirety of my Index.razor file

```html
@page "/"

@using BlazorTextEditor.RazorLib.ViewModel

<!-- Omitted Templated Blazor Markup for Conciseness -->

<TextEditorViewModelDisplay TextEditorViewModelKey="IndexTextEditorViewModelKey" />

<div>
    @@_savedContent:
    
    @* 
        I am only using a <pre> tag here to demonstrate the OnSaveRequested functionality.
        I believe <pre> tags can put you at risk of script injection similarly to a MarkupString if the text is from an untrusted source.
    *@
    <pre>
        @_savedContent
    </pre>
</div>

```

- Lastly here is a gif which showcases me saving the content of the text editor, then the saved content is displayed in the user interface.

![Render the Initializer component](/Images/Gifs/v7.0.0/save.gif)

# Next tutorial: [Settings](/Documentation/20_SETTINGS.md)