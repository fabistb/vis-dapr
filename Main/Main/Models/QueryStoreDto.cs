namespace Main.Models;

// Copy from StateStoreDto for whatever reason the age must be handled as double otherwise it returns an exception
//  ---> System.Text.Json.JsonException: The JSON value could not be converted to Main.Models.StateStoreDto. Path: $.age | LineNumber: 0 | BytePositionInLine: 35.
//  ---> System.FormatException: Either the JSON value is not in a supported format, or is out of bounds for an Int32.
//    at System.Text.Json.Utf8JsonReader.GetInt32()
public record QueryStoreDto(string FirstName, string LastName, double Age);