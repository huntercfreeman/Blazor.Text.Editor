namespace BlazorTextEditor.RazorLib;

public interface ITextEditorServiceOptions
{
    /// <summary>
    ///     If the consumer of the Nuget Package is
    ///     registering Fluxor themselves they can include
    ///     typeof(TextEditorOptions).Assembly when invoking
    ///     AddFluxor to add it as a service.
    ///     <br /><br />
    ///     As well the Fluxor.Blazor.Web.StoreInitializer will
    ///     not be rendered from within the Nuget Package
    /// </summary>
    public bool InitializeFluxor { get; }
    public bool UseLocalStorageForSettings { get; set; }
}