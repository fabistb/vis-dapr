namespace Main.Models;

public record CreateBlobDto(string FileName, BlobData Data);

public record BlobData(string Test);