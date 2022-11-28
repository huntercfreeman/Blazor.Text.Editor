using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;

public class JsonSyntaxWalker
{
    public List<JsonLineCommentSyntax> JsonLineCommentSyntaxes { get; } = new();

    public void Visit(IJsonSyntax jsonSyntax)
    {
        foreach (var child in jsonSyntax.ChildJsonSyntaxes) 
            Visit(child);

        switch (jsonSyntax.JsonSyntaxKind)
        {
            case JsonSyntaxKind.LineComment:
                VisitJsonLineCommentSyntax((JsonLineCommentSyntax)jsonSyntax);
                break;
        }
    }

    private void VisitJsonLineCommentSyntax(JsonLineCommentSyntax jsonSyntax)
    {
        JsonLineCommentSyntaxes.Add(jsonSyntax);
    }
}