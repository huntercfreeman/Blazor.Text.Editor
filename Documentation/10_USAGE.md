# Blazor.Text.Editor

## Usage

### Goal

- Modify Pages/Index.razor to render a C# Text Editor with Syntax Highlighting
- Modify Pages/Counter.razor to render a Razor Text Editor with Syntax Highlighting
- Modify Pages/FetchData.razor to render a Razor Text Editor with Syntax Highlighting
- Demonstrate the state management by navigating between the different pages and seeing the Text Editor state persist.
- Render 3 C# Text Editors to showcase the event dispatching as typing in one editor modifies the others.

### Steps
- One can write the upcoming code however they would like, I will be using the `@code` section in Razor markup.

- Add in `Pages/Index.razor` an `@code` section.

- Within the `@code` section proceed to inject the `ITextEditorService`.

- At this stage my Index.razor looks like the following:

```html
@page "/"
@using BlazorTextEditor.RazorLib

<PageTitle>Index</PageTitle>

<h1>Hello, world!</h1>

Welcome to your new app.

<SurveyPrompt Title="How is Blazor working for you?"/>

@code {
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
}
```

- In order to change pages and `maintain` the TextEditor's `state` the `TextEditorKey` which is used `must not change`.

- One can maintain a reference to an unchanging `TextEditorKey` in various ways. I will be making a static readonly `TextEditorKey` within the `@code` section of `Index.razor`.

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

- The `RegisterCSharpTextEditor` method wants us to provide as arguments: a `TextEditorKey`, and a `string to set` as the `initial text contents` of the text editor.

- I will invoke the method as shown in the following code snippet

```csharp
[Inject]
private ITextEditorService TextEditorService { get; set; } = null!;

private static readonly TextEditorKey IndexTextEditorKey = 
    TextEditorKey.NewTextEditorKey();

protected override void OnInitialized()
{
    TextEditorService.RegisterCSharpTextEditor(
        IndexTextEditorKey,
        string.Empty);
    
    base.OnInitialized();
}
```

- Add the following using statement to Index.razor if it is not already there.

```html
@using BlazorTextEditor.RazorLib
```

- Render to Index.razor a self closing Blazor component by the name of `TextEditorDisplay`. See the following code snippet

```html
<TextEditorDisplay/>
```

- The `TextEditorDisplay` Blazor component has many Blazor parameters available for customization.

- However, only one parameter is needed to render a `TextEditorDisplay` and that is `TextEditorKey`.

- Pass to the `TextEditorDisplay` the Blazor parameter named, `TextEditorKey` with the value of `IndexTextEditorKey`. See the following code snippet.

```html
<TextEditorDisplay TextEditorKey="IndexTextEditorKey"/>
```

- Now run the application and you will have rendered a `TextEditorDisplay` with C# syntax highlighting.

![Rendered C# Text Editor](/Images/Gifs/01_usage-rendered.gif)

