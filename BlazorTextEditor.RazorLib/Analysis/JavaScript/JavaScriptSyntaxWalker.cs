namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public class JavaScriptSyntaxWalker
{
    public List<JavaScriptStringSyntax> JavaScriptStringSyntaxes { get; } = new();
    public List<JavaScriptCommentSyntax> JavaScriptCommentSyntaxes { get; } = new();
    public List<JavaScriptKeywordSyntax> JavaScriptKeywordSyntaxes { get; } = new();

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
            case JavaScriptSyntaxKind.Comment:
                VisitJavaScriptCommentSyntax((JavaScriptCommentSyntax)node);
                break;
            case JavaScriptSyntaxKind.Keyword:
                VisitJavaScriptKeywordSyntax((JavaScriptKeywordSyntax)node);
                break;
        }
    }

    private void VisitJavaScriptStringSyntax(JavaScriptStringSyntax node)
    {
        JavaScriptStringSyntaxes.Add(node);
    }
    
    private void VisitJavaScriptCommentSyntax(JavaScriptCommentSyntax node)
    {
        JavaScriptCommentSyntaxes.Add(node);
    }
    
    private void VisitJavaScriptKeywordSyntax(JavaScriptKeywordSyntax node)
    {
        JavaScriptKeywordSyntaxes.Add(node);
    }
}