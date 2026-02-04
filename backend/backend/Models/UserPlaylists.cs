namespace backend.Models;

public record UserPlaylists(Provider Provider, IReadOnlyList<Playlist> Playlists);