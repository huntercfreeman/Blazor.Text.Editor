namespace BlazorTextEditor.RazorLib.Semantics;

public class SymbolDefinition
{
    public SymbolDefinition(
        string resourceUri,
        int positionIndex)
    {
        ResourceUri = resourceUri;
        PositionIndex = positionIndex;
    }

    public string ResourceUri { get; }
    public int PositionIndex { get; }
}