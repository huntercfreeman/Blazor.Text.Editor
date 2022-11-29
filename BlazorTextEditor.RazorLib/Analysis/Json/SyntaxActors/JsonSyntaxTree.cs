

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
                    
                    // Skip the delimiter between the property's key and value.
                    while (!stringWalker.IsEof)
                    {
                        _ = stringWalker.Consume();

                        if (JsonFacts.PROPERTY_DELIMITER_BETWEEN_KEY_AND_VALUE == stringWalker.CurrentCharacter)
                            break;
                    }
                }
            }
            else
            {
                if (JsonFacts.PROPERTY_VALUE_STRING_TEXT_STARTING == stringWalker.CurrentCharacter)
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
        // +1 to not include the quote that beings the key's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;
        
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
    /// <see cref="StringWalker.Backtrack"/> once to move
    /// position index to the starting quote.
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        int startingPositionIndex;

        if (stringWalker.CurrentCharacter == JsonFacts.PROPERTY_VALUE_STRING_TEXT_STARTING)
        {
            // +1 to not include the quote that beings this values's text
            startingPositionIndex = stringWalker.PositionIndex + 1;
        }
        else
        {
            startingPositionIndex = stringWalker.PositionIndex;
        }

        stringWalker.Backtrack();
        
        bool hasSkippedWhitespace = false;
        bool? valueIsTypeString = null;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (hasSkippedWhitespace)
            {
                if (valueIsTypeString is null)
                {
                    if (JsonFacts.PROPERTY_VALUE_STRING_TEXT_STARTING == stringWalker.CurrentCharacter)
                    {
                        valueIsTypeString = true;
                        continue;
                    }

                    valueIsTypeString = false;
                }
                else
                {
                    if (valueIsTypeString.Value)
                    {
                        if (JsonFacts.PROPERTY_VALUE_STRING_TEXT_ENDING == stringWalker.CurrentCharacter)
                            break;
                    }
                    else
                    {
                        if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                            break;
                    }
                }
            }
            else
            {
                if (!WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                {
                    hasSkippedWhitespace = true;
                    stringWalker.Backtrack();
                }
            }
        }

        if (JsonFacts.PROPERTY_VALUE_STRING_TEXT_ENDING != stringWalker.CurrentCharacter)
        {
            // TODO: The StartingIndexInclusive and EndingIndexExclusive for EOF are?
            textEditorJsonDiagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex - 1, 
                    stringWalker.PositionIndex,
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