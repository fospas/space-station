using Robust.Shared.Serialization;

namespace Content.Shared._Cats.Discord;

[Serializable, NetSerializable]
public sealed class DiscordSponsorInfo
{
    public SponsorTier[] Tiers { get; set; } = Array.Empty<SponsorTier>();
}
