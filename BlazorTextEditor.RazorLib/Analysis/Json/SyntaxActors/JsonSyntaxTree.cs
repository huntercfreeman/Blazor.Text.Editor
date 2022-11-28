

using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;

public class JsonSyntaxTree
{
    public static JsonSyntaxUnit ParseText(string content)
    {
        // Items to return wrapped in a JsonSyntaxUnit
        var jsonDocumentChildren = new List<IJsonSyntax>();
        var textEditorJsonDiagnosticBag = new TextEditorJsonDiagnosticBag();

        // Step through the string 'character by character'
        var stringWalker = new StringWalker(content);

        // Order matters with the methods of pattern, 'Consume{Something}'
        // Example: 'ConsumeComment'
        while (!stringWalker.IsEof)
        {
            // TODO: The following 'ConsumeIdentifier' invocation is just an example and needs replaced with actual lexing for JSON
            //
            // if (char.IsLetterOrDigit(stringWalker.CurrentCharacter))
            //     ConsumeIdentifier(stringWalker, jsonDocumentChildren, textEditorJsonDiagnosticBag);

            _ = stringWalker.Consume();
        }

        var jsonDocumentSyntax = new JsonDocumentSyntax(
            new TextEditorTextSpan(
                0,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None),
            jsonDocumentChildren.ToImmutableArray());

        var jsonSyntaxUnit = new JsonSyntaxUnit(
            jsonDocumentSyntax,
            textEditorJsonDiagnosticBag);

        return jsonSyntaxUnit;
    }
}