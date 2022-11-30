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

            if (JsonFacts.PROPERTY_LIST_DELIMITER == stringWalker.CurrentCharacter)
                continue;

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
            // This is to mean the property value is
            // invalid in some regard
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
        
        if (stringWalker.IsEof)
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
    /// - <see cref="JsonFacts.PROPERTY_KEY_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_KEY_END"/><br/>
    /// </summary>
    private static JsonPropertyKeySyntax ConsumePropertyKey(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        // +1 to not include the quote that begins the key's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.Consume();

            if (JsonFacts.PROPERTY_KEY_END == stringWalker.CurrentCharacter)
                break;
        }

        if (stringWalker.IsEof)
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
    /// - Any character that is not <see cref="WhitespaceFacts.ALL"/> (whitespace)<br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.PROPERTY_LIST_DELIMITER"/><br/>
    /// - <see cref="WhitespaceFacts.ALL"/> (whitespace)<br/>
    /// - The <see cref="JsonFacts.OBJECT_END"/> of the object which contains the property value<br/>
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        int startingPositionIndex = stringWalker.PositionIndex;
        JsonSyntaxKind jsonSyntaxKindOfValue;

        IJsonSyntax underlyingJsonSyntax;
        
        if (stringWalker.CurrentCharacter == JsonFacts.ARRAY_START)
        {
            underlyingJsonSyntax = ConsumeArray(
                stringWalker, 
                textEditorJsonDiagnosticBag);
        }
        else if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
        {
            underlyingJsonSyntax = ConsumeObject(
                stringWalker, 
                textEditorJsonDiagnosticBag);
        }
        else
        {
            if (stringWalker.CurrentCharacter == JsonFacts.STRING_START)
            {
                underlyingJsonSyntax = ConsumeString(
                    stringWalker, 
                    textEditorJsonDiagnosticBag);
            }
            else
            {
                underlyingJsonSyntax = ConsumeAmbiguousValue(
                    stringWalker, 
                    textEditorJsonDiagnosticBag);
            }
        }

        if (stringWalker.IsEof)
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
            underlyingJsonSyntax);

        return jsonPropertyValue;
    }
    
    private static JsonPropertyValueSyntax ConsumeArray(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        throw new NotImplementedException();
    }
    
    private static JsonPropertyValueSyntax ConsumeObject(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        throw new NotImplementedException();
    }
    
    private static JsonPropertyValueSyntax ConsumeString(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        
        // +1 to not include the quote that beings this values's text
        startingPositionIndex = stringWalker.PositionIndex + 1;
    }
    
    /// <summary>
    /// The JSON DataTypes which qualify as ambiguous are:<br/>
    /// -number<br/>
    /// -integer<br/>
    /// -boolean<br/>
    /// -null<br/>
    /// <br/>
    /// One must ensure the value cannot be the following
    /// DataTypes prior to invoking this method:<br/>
    /// -array<br/>
    /// -object<br/>
    /// -string<br/>
    /// </summary>
    private static JsonPropertyValueSyntax ConsumeAmbiguousValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        throw new NotImplementedException();
    }
}