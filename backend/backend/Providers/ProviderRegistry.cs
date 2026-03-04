using backend.Models;

namespace backend.Providers;

public static class ProviderRegistry
{
    public static readonly IReadOnlyDictionary<Provider, IProvider> All = 
        new Dictionary<Provider, IProvider>
        {
            { Provider.Spotify, new SpotifyAPI() },
            { Provider.YouTubeMusic, new YoutubeAPI() }
        };

    public static bool TryGet(Provider provider, out IProvider handler) 
        => All.TryGetValue(provider, out handler!);
}