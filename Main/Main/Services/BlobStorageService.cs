using System.Text;
using System.Text.Json;
using Dapr.Client;
using Main.Models;
using Microsoft.Toolkit.HighPerformance;

namespace Main.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly DaprClient _daprClient;

    public BlobStorageService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<string> Create(CreateBlobDto request)
    {
        var bindingRequest = new BindingRequest("storagebinding", "create")
        {
            Data = JsonSerializer.SerializeToUtf8Bytes(request.Data),
            Metadata = 
            {
                {"blobName", request.FileName}
            }
        };

        var blobResponse = await _daprClient.InvokeBindingAsync(bindingRequest);
        var response = await JsonSerializer.DeserializeAsync<BlobStorageResponseDto>(blobResponse.Data.AsStream());
        return response.BlobUrl;
    }

    public async Task<object?> Get(string blobName, bool metadata)
    {
        var bindingRequest = new BindingRequest("storagebinding", "get")
        {
            Data = null,
            Metadata =
            {
                {"blobName", blobName},
                {"includeMetadata", metadata.ToString().ToLower()}
            }
        };

        var blobResponse = await _daprClient.InvokeBindingAsync(bindingRequest);
        return await JsonSerializer.DeserializeAsync<object>(blobResponse.Data.AsStream());
    }

    public async Task<object?> List(BlobStorageListDto request)
    {
        var bindingRequest = new BindingRequest("storagebinding", "list")
        {
            Data = JsonSerializer.SerializeToUtf8Bytes(request),
        };

        var blobResponse = await _daprClient.InvokeBindingAsync(bindingRequest);

        return await JsonSerializer.DeserializeAsync<object>(blobResponse.Data.AsStream());
    }

    public async Task Delete(string blobName)
    {
        var bindingRequest = new BindingRequest("storagebinding", "delete")
        {
            Data = null,
            Metadata =
            {
                {"blobName", blobName},
            }
        };

        await _daprClient.InvokeBindingAsync(bindingRequest);
    }
}