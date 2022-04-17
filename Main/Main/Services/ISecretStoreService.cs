namespace Main.Services;

public interface ISecretStoreService
{
    Task<string> GetSecretBySecretName(string secretName);
}