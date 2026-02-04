namespace backend.Models;

public record UserQuery(IReadOnlyList<SearchQuery> SearchQueries, IReadOnlyList<UserPlaylists> UserPlaylists);