namespace backend.Models;

public record SearchRequest(string TrackName, IReadOnlyList<ProviderAccess> SelectedProviders);