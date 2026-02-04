namespace backend.Models;

public record SessionContext(string Username, IReadOnlyList<ProviderAccess> Providers);