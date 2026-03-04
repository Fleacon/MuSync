using backend.Models;

namespace backend.Providers;

public class ProviderRegistry
{
    public IReadOnlyDictionary<Provider, IProvider> All { get; }

    public ProviderRegistry(SoundCloudAPI soundCloud)
    {
        All = new Dictionary<Provider, IProvider>
        {
            { Provider.Spotify, new SpotifyAPI() },
            { Provider.YouTubeMusic, new YoutubeAPI() },
            { Provider.SoundCloud, soundCloud }  // injected, not newed
        };
    }

    public bool TryGet(Provider provider, out IProvider handler)
        => All.TryGetValue(provider, out handler!);
}