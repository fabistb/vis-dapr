using System.Text;
using Dapr.Client;
using Main.Models;

namespace Main.Services;

public class CryptographyService : ICryptographyService
{
    private readonly DaprClient _daprClient;

    public CryptographyService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }
    
    public async Task<string> Encrypt(CryptographyDto cryptographyDto)
    {
        var secretTextBytes = Encoding.UTF8.GetBytes(cryptographyDto.SecretMessage);
        var encryptedText = await _daprClient.EncryptAsync(
            "cryptography", 
            secretTextBytes, 
            "cryptokey.pem",
            new EncryptionOptions(KeyWrapAlgorithm.Rsa));

        return Convert.ToBase64String(encryptedText.Span);
    }

    public async Task<string> Decrypt(CryptographyDto cryptographyDto)
    {
        var base64Decoded = Convert.FromBase64String(cryptographyDto.SecretMessage);
        var decryptedByte = await _daprClient.DecryptAsync(
            "cryptography", 
            base64Decoded,
            "cryptokey.pem");
        
        return Encoding.UTF8.GetString(decryptedByte.ToArray());
    }
}