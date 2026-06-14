namespace LLMFlexWin.Storage;

public interface ISecretStore
{
    void SetKey(Guid profileId, string apiKey);
    string? GetKey(Guid profileId);
    bool HasKey(Guid profileId);
    void DeleteKey(Guid profileId);
}