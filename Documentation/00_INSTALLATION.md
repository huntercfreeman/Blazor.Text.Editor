# Blazor.Text.Editor
*NOTE*: Last Modified: (v7.1.0)

## Installation

### Goal

- Reference the `Blazor.Text.Editor` Nuget Package
- Register the `Services`
- Reference the `CSS`
- Reference the `JavaScript`
- In App.razor render the &lt;`Fluxor.Blazor.Web.StoreInitializer`/&gt;
- In MainLayout.razor render both &lt;`BlazorCommon.RazorLib.BlazorCommonInitializer`/&gt; and &lt;`BlazorTextEditor.RazorLib.BlazorTextEditorInitializer`/&gt;.

### Steps
- Reference the `Blazor.Text.Editor` NuGet Package

Use your preferred way to install NuGet Packages to install v7.1.0 of `Blazor.Text.Editor`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Blazor.Text.Editor

- Register the Services

In my C# Project services are registered in Program.cs

Go to the file that you register your services and add the following lines of C# code.

```csharp
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.WatchWindow;
using BlazorCommon.RazorLib.WatchWindow.TreeViewDisplays;
using BlazorTextEditor.RazorLib;

// Blazor.Text.Editor
{
    var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
        typeof(TreeViewTextDisplay),
        typeof(TreeViewReflectionDisplay),
        typeof(TreeViewPropertiesDisplay),
        typeof(TreeViewInterfaceImplementationDisplay),
        typeof(TreeViewFieldsDisplay),
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewEnumerableDisplay));
        
    var commonRendererTypes = new BlazorCommonComponentRenderers(
        typeof(BackgroundTaskDisplay),
        typeof(CommonErrorNotificationDisplay),
        typeof(CommonInformativeNotificationDisplay),
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewMissingRendererFallbackDisplay),
        watchWindowTreeViewRenderers);
    
    builder.Services.AddSingleton<IBlazorCommonComponentRenderers>(_ => commonRendererTypes);
    builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
    builder.Services.AddSingleton<IBackgroundTaskMonitor, BackgroundTaskMonitor>();
    builder.Services.AddHostedService<QueuedHostedService>();
    builder.Services.AddBlazorTextEditor();
}
```

- Reference the CSS

In my C# Project CSS files are referenced from Pages/_Layout.cshtml

Go to the file that you reference CSS files from and add the following CSS references.

```html
<!-- Blazor.Common | Required CSS -->
<link href="_content/Blazor.Common/blazorCommon.css" rel="stylesheet" />
<!-- Blazor.Text.Editor | Required CSS -->
<link href="_content/Blazor.Text.Editor/blazorTextEditor.css" rel="stylesheet" />
```

- Reference the JavaScript

In my C# Project JavaScript files are referenced from Pages/_Layout.cshtml

Go to the file that you reference JavaScript files from and add the following JavaScript reference below the Blazor framework JavaScript reference

```html
<!-- Blazor.Common | Required CSS -->
<script src="_content/Blazor.Common/blazorCommon.js"></script>
<!-- Blazor.Text.Editor | Required CSS -->
<script src="_content/Blazor.Text.Editor/blazorTextEditor.js"></script>
```

- In App.razor render the &lt;`Fluxor.Blazor.Web.StoreInitializer`/&gt;

In App.razor, where the Blazor Router is located, at the top of the file add the following Razor markup

*NOTE*: [Fluxor](https://github.com/mrpmorris/Fluxor) is a state management library which `Blazor.Text.Editor` uses internally.

```html
<Fluxor.Blazor.Web.StoreInitializer/>
```

- In MainLayout.razor render both &lt;`BlazorCommon.RazorLib.BlazorCommonInitializer`/&gt; and &lt;`BlazorTextEditor.RazorLib.BlazorTextEditorInitializer`/&gt;.

In MainLayout.razor, within the top most level HTML `element`, as child content add the following Razor markup

```html
<BlazorCommon.RazorLib.BlazorCommonInitializer/>
<BlazorTextEditor.RazorLib.BlazorTextEditorInitializer/>
```

# Next tutorial: [Usage](/Documentation/10_USAGE.md)