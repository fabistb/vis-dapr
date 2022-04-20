namespace Main.Services;

public interface ISecretStoreService
{
    Task<string> GetSecretBySecretName(string secretName);

    Task<Dictionary<string, Dictionary<string, string>>> GetSecrets();
}