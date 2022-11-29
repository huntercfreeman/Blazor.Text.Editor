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
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.OBJECT_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.OBJECT_END"/><br/>
    /// </summary>
    private static void ConsumeObject(
        StringWalker stringWalker, 
        List<IJsonSyntax> jsonDocumentChildren,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        var jsonPropertySyntaxes = new List<JsonPropertySyntax>();

        // While loop state
        JsonPropertyKeySyntax? pendingJsonPropertyKeySyntax = null;
        var foundPropertyDelimiterBetweenKeyAndValue = false;
        JsonPropertyValueSyntax? pendingJsonPropertyValueSyntax = null;
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            // Skip whitespace
            while (!stringWalker.IsEof)
            {
                if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                    _ = stringWalker.Consume();
                else
                    break;
            }
            
            if (JsonFacts.OBJECT_END == stringWalker.CurrentCharacter)
                break;

            if (pendingJsonPropertyKeySyntax is null)
            {
                pendingJsonPropertyKeySyntax = ConsumePropertyKey(
                    stringWalker,
                    textEditorJsonDiagnosticBag);
            }
            else if (!foundPropertyDelimiterBetweenKeyAndValue)
            {
                while (!stringWalker.IsEof)
                {
                    if (JsonFacts.PROPERTY_DELIMITER_BETWEEN_KEY_AND_VALUE != stringWalker.CurrentCharacter)
                        _ = stringWalker.Consume();
                    else
                        break;
                }

                // If Eof ended the loop to find the delimiter
                // the outer while loop will finish as well so
                // no EOF if is needed just set found to true
                foundPropertyDelimiterBetweenKeyAndValue = true;
            }
            else
            {
                pendingJsonPropertyValueSyntax = ConsumePropertyValue(
                    stringWalker,
                    textEditorJsonDiagnosticBag);
                
                var jsonPropertySyntax = new JsonPropertySyntax(
                    new TextEditorTextSpan(
                        startingPositionIndex,
                        stringWalker.PositionIndex,
                        (byte)JsonDecorationKind.PropertyKey),
                    pendingJsonPropertyKeySyntax,
                    pendingJsonPropertyValueSyntax);

                // Reset while loop state
                pendingJsonPropertyKeySyntax = null;
                foundPropertyDelimiterBetweenKeyAndValue = false;
                pendingJsonPropertyValueSyntax = null;
                
                jsonPropertySyntaxes.Add(jsonPropertySyntax);
            }
        }

        if (pendingJsonPropertyKeySyntax is not null)
        {
            // This is to mean the property value invalid
            // in one of various ways
            //
            // Still however, render the syntax highlighting
            // for the valid property key.
            
            var jsonPropertySyntax = new JsonPropertySyntax(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.PropertyKey),
                pendingJsonPropertyKeySyntax,
                JsonPropertyValueSyntax.GetInvalidJsonPropertyValueSyntax());
            
            jsonPropertySyntaxes.Add(jsonPropertySyntax);
        }
        
        if (JsonFacts.OBJECT_END != stringWalker.CurrentCharacter)
        {
            textEditorJsonDiagnosticBag.ReportEndOfFileUnexpected(
                new TextEditorTextSpan(
                    stringWalker.PositionIndex, 
                    stringWalker.PositionIndex + 1,
                    (byte)JsonDecorationKind.Error));
        }
        
        var jsonObjectSyntax = new JsonObjectSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.PropertyValue),
            jsonPropertySyntaxes.ToImmutableArray());
            
        jsonDocumentChildren.Add(jsonObjectSyntax);
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.PROPERTY_KEY_TEXT_STARTING"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_KEY_TEXT_ENDING"/><br/>
    /// </summary>
    private static JsonPropertyKeySyntax ConsumePropertyKey(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        // +1 to not include the quote that begins the key's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;
        
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
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.PROPERTY_DELIMITER_BETWEEN_KEY_AND_VALUE"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_LIST_DELIMITER"/><br/>
    /// - <see cref="WhitespaceFacts.ALL"/><br/>
    /// - The <see cref="JsonFacts.OBJECT_END"/> of the object which contains the property value<br/>
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        if (JsonFacts.PROPERTY_VALUE_STRING_TEXT_STARTING == stringWalker.CurrentCharacter)
        {
            pendingJsonPropertyValueSyntax = ConsumePropertyValue(
                stringWalker,
                textEditorJsonDiagnosticBag);
        }
        
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