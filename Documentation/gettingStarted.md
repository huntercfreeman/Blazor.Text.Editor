# Blazor.Text.Editor - documentation

## Installation and Setup

### Goal
- This tutorial will use the default Blazor ServerSide template.
- A button is put on the Index.razor page. When this button is clicked a text editor instance will be registered.
- By means of an event handler the Index.razor will be notified to rerender whenever a text editor instance is either registered or disposed.
- The Index.razor will re-render upon receiving a notification that a text editor instance was either registered or disposed.

### Steps
- Add a NuGet package reference to `Blazor.Text.Editor`

- Register library's services by invoking the IServiceCollection extension method `AddTextEditorRazorLibServices()`

```csharp

// In Program.cs

using BlazorTextEditor.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// ... using elipses for conciseness

builder.Services.AddTextEditorRazorLibServices();

var app = builder.Build();
```

Edit `App.razor` and app the following markup
```html
<Fluxor.Blazor.Web.StoreInitializer/>
```
