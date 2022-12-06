using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Json.Facts;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxObjects;
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
                var jsonObjectSyntax = ConsumeObject(
                    stringWalker, 
                    textEditorJsonDiagnosticBag);

                jsonDocumentChildren.Add(jsonObjectSyntax);
            }
            else if (stringWalker.CurrentCharacter == JsonFacts.ARRAY_START)
            {
                var jsonObjectSyntax = ConsumeArray(
                    stringWalker, 
                    textEditorJsonDiagnosticBag);

                jsonDocumentChildren.Add(jsonObjectSyntax);
            }

            _ = stringWalker.ReadCharacter();
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
    private static JsonObjectSyntax ConsumeObject(
        StringWalker stringWalker,
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
            _ = stringWalker.ReadCharacter();

            // Skip whitespace
            while (!stringWalker.IsEof)
            {
                if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                    _ = stringWalker.ReadCharacter();
                else
                    break;
            }
            
            if (JsonFacts.OBJECT_END == stringWalker.CurrentCharacter)
                break;

            if (JsonFacts.PROPERTY_ENTRY_DELIMITER == stringWalker.CurrentCharacter)
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
                        _ = stringWalker.ReadCharacter();
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
                (byte)JsonDecorationKind.None),
            jsonPropertySyntaxes.ToImmutableArray());
            
        return jsonObjectSyntax;
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
            _ = stringWalker.ReadCharacter();

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
    /// - <see cref="JsonFacts.PROPERTY_ENTRY_DELIMITER"/><br/>
    /// - <see cref="WhitespaceFacts.ALL"/> (whitespace)<br/>
    /// - The <see cref="JsonFacts.OBJECT_END"/> of the object which contains the property value<br/>
    /// </summary>
    private static JsonPropertyValueSyntax ConsumePropertyValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        int startingPositionIndex = stringWalker.PositionIndex;

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
                (byte)JsonDecorationKind.None),
            underlyingJsonSyntax);

        return jsonPropertyValue;
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.ARRAY_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.ARRAY_END"/><br/>
    /// </summary>
    private static JsonArraySyntax ConsumeArray(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        // +1 to not include the bracket that begins this values's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        var jsonObjectSyntaxes = new List<JsonObjectSyntax>();
        
        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            // Skip whitespace
            while (!stringWalker.IsEof)
            {
                if (WhitespaceFacts.ALL.Contains(stringWalker.CurrentCharacter))
                    _ = stringWalker.ReadCharacter();
                else
                    break;
            }
            
            if (JsonFacts.ARRAY_END == stringWalker.CurrentCharacter)
                break;

            if (JsonFacts.ARRAY_ENTRY_DELIMITER == stringWalker.CurrentCharacter)
                continue;
            
            if (stringWalker.CurrentCharacter == JsonFacts.OBJECT_START)
            {
                var jsonObjectSyntax = ConsumeObject(
                    stringWalker, 
                    textEditorJsonDiagnosticBag);

                jsonObjectSyntaxes.Add(jsonObjectSyntax);
            }
        }

        return new JsonArraySyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.None),
            jsonObjectSyntaxes.ToImmutableArray());
    }
    
    /// <summary>
    /// currentCharacterIn:<br/>
    /// - <see cref="JsonFacts.STRING_START"/><br/>
    /// <br/>
    /// currentCharacterOut:<br/>
    /// - <see cref="JsonFacts.STRING_END"/><br/>
    /// </summary>
    private static JsonStringSyntax ConsumeString(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        // +1 to not include the quote that begins this values's text
        var startingPositionIndex = stringWalker.PositionIndex + 1;

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();

            if (JsonFacts.STRING_END == stringWalker.CurrentCharacter)
                break;
        }

        return new JsonStringSyntax(
            new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.String));
    }
    
    /// <summary>
    /// The JSON DataTypes which qualify as ambiguous are:<br/>
    /// -number<br/>
    /// -integer<br/>
    /// -boolean<br/>
    /// -null<br/>
    /// <br/>
    /// One must ensure the value cannot be of the following
    /// DataTypes prior to invoking this method:<br/>
    /// -array<br/>
    /// -object<br/>
    /// -string<br/>
    /// </summary>
    private static IJsonSyntax ConsumeAmbiguousValue(
        StringWalker stringWalker,
        TextEditorJsonDiagnosticBag textEditorJsonDiagnosticBag)
    {
        var startingPositionIndex = stringWalker.PositionIndex;

        var firstWordTuple = stringWalker.ConsumeWord(new []
        {
            ','
        }.ToImmutableArray());

        if (JsonFacts.NULL_STRING_VALUE == firstWordTuple.value)
        {
            return new JsonNullSyntax(
                new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.Keyword));
        }
        else if (JsonFacts.BOOLEAN_ALL_STRING_VALUES.Contains(firstWordTuple.value))
        {
            return new JsonBooleanSyntax(new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.Keyword));
        }
        else
        {
            if (firstWordTuple.value.Contains(JsonFacts.NUMBER_DECIMAL_PLACE_SEPARATOR))
            {
                return new JsonNumberSyntax(new TextEditorTextSpan(
                    startingPositionIndex,
                    stringWalker.PositionIndex,
                    (byte)JsonDecorationKind.Number));
            }

            return new JsonIntegerSyntax(new TextEditorTextSpan(
                startingPositionIndex,
                stringWalker.PositionIndex,
                (byte)JsonDecorationKind.Integer));
        }
    }
}

