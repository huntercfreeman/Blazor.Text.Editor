namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxWalker
{
    public List<JavaScriptStringSyntax> JavaScriptStringSyntaxes { get; } = new();

    public void Visit(IJavaScriptSyntax node)
    {
        foreach (var child in node.Children)
        {
            Visit(child);
        }

        switch (node.JavaScriptSyntaxKind)
        {
            case JavaScriptSyntaxKind.String:
                VisitJavaScriptStringSyntax((JavaScriptStringSyntax)node);
                break;
        }
    }

    private void VisitJavaScriptStringSyntax(JavaScriptStringSyntax node)
    {
        JavaScriptStringSyntaxes.Add(node);
    }
}