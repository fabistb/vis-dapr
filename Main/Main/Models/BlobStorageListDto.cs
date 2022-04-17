namespace Main.Models;

public record BlobStorageListDto(int MaxResults, string? Prefix, string? Marker, BlobStorageListAdditionalDataDto? Include);

public record BlobStorageListAdditionalDataDto(bool? MetaData);