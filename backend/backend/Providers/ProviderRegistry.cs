using backend.Models;

namespace backend.Providers;

public class ProviderRegistry
{
    public IReadOnlyDictionary<Provider, IProvider> All { get; }

    public ProviderRegistry(IHttpClientFactory httpClientFactory)
    {
        var providers = new Dictionary<Provider, IProvider>();

        if (SpotifyAPI.TryCreate(out var spotify))
            providers[Provider.Spotify] = spotify!;
        if (YoutubeAPI.TryCreate(out var youtube))
            providers[Provider.YouTubeMusic] = youtube!;
        if (SoundCloudAPI.TryCreate(httpClientFactory.CreateClient(), out var soundCloud))
            providers[Provider.SoundCloud] = soundCloud!;

        All = providers;
    }

    public bool TryGet(Provider provider, out IProvider handler)
        => All.TryGetValue(provider, out handler!);
}