namespace backend.Models;

public record Selections(IReadOnlyList<ProviderAccess> SelectedProviders, IReadOnlyList<SongSelection> SongSelections, IReadOnlyList<UserPlaylists> PlaylistSelections);