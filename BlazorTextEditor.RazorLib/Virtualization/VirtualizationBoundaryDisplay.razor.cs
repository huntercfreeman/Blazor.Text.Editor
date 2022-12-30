using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Virtualization;

public partial class VirtualizationBoundaryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizationBoundary VirtualizationBoundary { get; set; } = null!;
    [Parameter, EditorRequired]
    public string VirtualizationBoundaryDisplayId { get; set; } = null!;

    private string GetStyleCssString()
    {
        var styleBuilder = new StringBuilder();

        // Width
        {
            if (VirtualizationBoundary.WidthInPixels is null)
            {
                styleBuilder.Append(" width: 100%;");
            }
            else
            {
                var widthInPixelsInvariantCulture = VirtualizationBoundary.WidthInPixels.Value
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                styleBuilder.Append($" width: {widthInPixelsInvariantCulture}px;");
            }
        }

        // Height
        {
            if (VirtualizationBoundary.HeightInPixels is null)
            {
                styleBuilder.Append(" height: 100%;");
            }
            else
            {
                var heightInPixelsInvariantCulture = VirtualizationBoundary.HeightInPixels.Value
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                styleBuilder.Append($" height: {heightInPixelsInvariantCulture}px;");
            }
        }

        // Left
        {
            if (VirtualizationBoundary.LeftInPixels is null)
            {
                styleBuilder.Append(" left: 100%;");
            }
            else
            {
                var leftInPixelsInvariantCulture = VirtualizationBoundary.LeftInPixels.Value
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                styleBuilder.Append($" left: {leftInPixelsInvariantCulture}px;");
            }
        }

        // Top
        {
            if (VirtualizationBoundary.TopInPixels is null)
            {
                styleBuilder.Append(" top: 100%;");
            }
            else
            {
                var topInPixelsInvariantCulture = VirtualizationBoundary.TopInPixels.Value
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                styleBuilder.Append($" top: {topInPixelsInvariantCulture}px;");
            }
        }

        return styleBuilder.ToString();
    }
}