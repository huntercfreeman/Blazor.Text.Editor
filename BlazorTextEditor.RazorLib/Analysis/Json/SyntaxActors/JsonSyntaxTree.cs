

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
            if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
            {
                ConsumeObject(stringWalker, jsonDocumentChildren, textEditorJsonDiagnosticBag);
            }
            
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
    
    /// <summary>
    /// <see cref="ConsumeObject"/> will immediately invoke
    /// <see cref="StringWalker.Consume"/> once
    ///  invoked.
    /// </summary>
    private static void ConsumeObject(
        StringWalker stringWalker, 
        List<IJsonSyntax> jsonDocumentChildren,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        var jsonPropertySyntaxes = new List<JsonPropertySyntax>();

        JsonPropertyKeySyntax? pendingJsonPropertyKeySyntax = null;
        JsonPropertyValueSyntax? pendingJsonPropertyValueSyntax = null;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (JsonFacts.OBJECT_END == stringWalker.CurrentCharacter)
                break;

            if (pendingJsonPropertyKeySyntax is null)
            {
                if (JsonFacts.PROPERTY_KEY_TEXT_STARTING == stringWalker.CurrentCharacter)
                {
                    pendingJsonPropertyKeySyntax = ConsumePropertyKey(
                        stringWalker,
                        textEditorJsonDiagnosticBag);
                }
            }
            else
            {
                if (JsonFacts.PROPERTY_VALUE_TEXT_STARTING == stringWalker.CurrentCharacter)
                {
                    pendingJsonPropertyValueSyntax = ConsumePropertyValue(
                        stringWalker,
                        textEditorJsonDiagnosticBag);
                }
            }

            if (pendingJsonPropertyKeySyntax is not null &&
                pendingJsonPropertyValueSyntax is not null)
            {
                var jsonPropertySyntax = new JsonPropertySyntax(
                    new TextEditorTextSpan(
                        startingPositionIndex,
                        stringWalker.PositionIndex,
                        (byte)JsonDecorationKind.PropertyKey),
                    pendingJsonPropertyKeySyntax,
                    pendingJsonPropertyValueSyntax);
                
                jsonPropertySyntaxes.Add(jsonPropertySyntax);
            }
        }

        if (JsonFacts.OBJECT_END == stringWalker.CurrentCharacter)
        {
            var jsonObjectSyntax = new JsonObjectSyntax(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.PropertyValue),
                jsonPropertySyntaxes.ToImmutableArray());
            
            jsonDocumentChildren.Add(jsonObjectSyntax);
        }
        else
        {
            textEditorJsonDiagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex, 
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error));
        }
    }
    
    /// <summary>
    /// <see cref="ConsumePropertyKey"/> will immediately invoke
    /// <see cref="StringWalker.Consume"/> once
    ///  invoked.
    /// </summary>
    private static JsonPropertyKeySyntax ConsumePropertyKey(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (JsonFacts.PROPERTY_KEY_TEXT_ENDING == stringWalker.CurrentCharacter)
                break;
        }

        if (JsonFacts.PROPERTY_KEY_TEXT_ENDING != stringWalker.CurrentCharacter)
        {
            textEditorJsonDiagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex, 
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error));
        }
        
        var jsonPropertyKey = new JsonPropertyKeySyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.PropertyKey),
            ImmutableArray<IJsonSyntax>.Empty);

        return jsonPropertyKey;
    }
    
    /// <summary>
    /// <see cref="ConsumePropertyValue"/> will immediately invoke
    /// <see cref="StringWalker.Consume"/> once
    ///  invoked.
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (JsonFacts.PROPERTY_VALUE_TEXT_ENDING == stringWalker.CurrentCharacter)
                break;
        }

        if (JsonFacts.PROPERTY_VALUE_TEXT_ENDING != stringWalker.CurrentCharacter)
        {
            textEditorJsonDiagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex, 
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error));
        }
        
        var jsonPropertyValue = new JsonPropertyValueSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.PropertyValue),
            null);

        return jsonPropertyValue;
    }
}