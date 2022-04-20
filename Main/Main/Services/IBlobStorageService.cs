using Main.Models;

namespace Main.Services;

public interface IBlobStorageService
{
    Task<string> Create(CreateBlobDto request);

    Task<object?> Get(string blobName, bool metadata);

    Task<object?> List(BlobStorageListDto request);

    Task Delete(string blobName);
}