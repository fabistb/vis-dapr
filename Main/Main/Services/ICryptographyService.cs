using Main.Models;

namespace Main.Services;

public interface ICryptographyService
{
    Task<string> Encrypt(CryptographyDto cryptographyDto);
    
    Task<string> Decrypt(CryptographyDto cryptographyDto);
}