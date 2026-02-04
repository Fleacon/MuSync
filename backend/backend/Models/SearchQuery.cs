namespace backend.Models;

public record SearchQuery(Provider Provider, IReadOnlyList<Track> Tracks);