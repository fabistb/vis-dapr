using System.Text.Json.Serialization;

namespace Main.Models;

public class BlobStorageResponseDto
{
    [JsonPropertyName("blobURL")]
    public string BlobUrl { get; set; }
}