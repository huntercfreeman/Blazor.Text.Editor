# Blazor.Text.Editor

## Installation

### Goal

- Reference the `Blazor.Text.Editor` Nuget Package
- Register the Services
- Reference the CSS
- Reference the JavaScript
- In App.razor render the <BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/> Blazor component

### Steps
- Reference the `Blazor.Text.Editor` NuGet Package

Use your preferred way to install NuGet Packages to install v6.1.0 of `Blazor.Text.Editor`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Blazor.Text.Editor

The following is a gif of me installing the NuGet Package

![Reference the NuGet Package](/Images/Gifs/00_nuget-package.gif)

- Register the Services

In my C# Project services are registered in Program.cs

Go to the file that you register your services and add the following lines of C# code.

```csharp
using BlazorTextEditor.RazorLib;

builder.Services.AddBlazorTextEditor();
```

The following is a gif of me registering the Services

![Register the Services](/Images/Gifs/00_register-services.gif)

- Reference the CSS

In my C# Project CSS files are referenced from Pages/_Layout.cshtml

Go to the file that you reference CSS files from and add the following CSS references.

```html
    <!-- Blazor.Text.Editor | Required CSS -->
    <link href="_content/BlazorALaCarte.Shared/blazorShared.css" rel="stylesheet"/>
    <link href="_content/BlazorALaCarte.DialogNotification/blazorDialogNotification.css" rel="stylesheet"/>
    <link href="_content/BlazorALaCarte.TreeView/blazorTreeView.css" rel="stylesheet"/>
    <link href="_content/Blazor.Text.Editor/blazorTextEditor.css" rel="stylesheet"/>
    <link href="_content/Blazor.Text.Editor/blazorTextEditorSizes.css" rel="stylesheet"/>

    <!-- Blazor.Text.Editor | Theme: Visual Studio Dark Clone (:root level, no css class needed) -->
    <link href="_content/Blazor.Text.Editor/blazorTextEditorDefaultColors.css" rel="stylesheet"/>
    <!-- Blazor.Text.Editor | Theme: Visual Studio Light Clone -->
    <link href="_content/Blazor.Text.Editor/Themes/blazorTextEditorVisualStudioLightTheme.css" rel="stylesheet"/>
```

The following is a gif of me referencing the CSS (the exact text pasted in this gif is outdated)

![Reference the CSS](/Images/Gifs/00_reference-css.gif)

- Reference the JavaScript

In my C# Project JavaScript files are referenced from Pages/_Layout.cshtml

Go to the file that you reference JavaScript files from and add the following JavaScript reference below the Blazor framework JavaScript reference

```html
    <!-- Blazor.Text.Editor | Required JavaScript -->
    <script src="_content/BlazorALaCarte.Shared/blazorShared.js"></script>
    <script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>
```

The following is a gif of me referencing the JavaScript (the exact text pasted in this gif is outdated)

![Reference the JavaScript](/Images/Gifs/00_reference-js.gif)

- In App.razor render the <BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/> Blazor component

In App.razor, where the Blazor Router is located, at the top of the file add the following Razor markup

```html
<BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/>
```

The following is a gif of me rendering the `<BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/>` Blazor component in App.razor

![Render the Initializer component](/Images/Gifs/00_initializer-component.gif)

# Next tutorial: [Usage](/Documentation/10_USAGE.md)