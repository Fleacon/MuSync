namespace backend.Models;

public record SessionContext(string Username, IReadOnlyList<Provider> Providers);