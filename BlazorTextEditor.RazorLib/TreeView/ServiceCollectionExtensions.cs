using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib.TreeView;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTreeView(
        this IServiceCollection services,
        Action<BlazorTreeViewOptions>? configure = null)
    {
        var blazorTreeViewOptions = new BlazorTreeViewOptions();
        configure?.Invoke(blazorTreeViewOptions);

        services
            .AddSingleton<IBlazorTreeViewOptions, ImmutableBlazorTreeViewOptions>(
                _ => new ImmutableBlazorTreeViewOptions(blazorTreeViewOptions))
            .AddScoped<ITreeViewService, TreeViewService>();

        if (blazorTreeViewOptions.InitializeFluxor)
        {
            services
                .AddFluxor(options => options
                    .ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly));
        }

        return services;
    }
}