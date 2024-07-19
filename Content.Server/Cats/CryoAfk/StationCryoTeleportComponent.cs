using Robust.Shared.Audio;
using Robust.Shared.Network;

namespace Content.Server.Cats.CryoTeleport;

[RegisterComponent]
public sealed partial class StationCryoTeleportComponent : Component
{
    [DataField]
    public TimeSpan TransferDelay = TimeSpan.FromSeconds(600);  // TODO: debug

    [DataField]
    public string PortalPrototype = "PortalCryo";

    [DataField]
    public SoundSpecifier TransferSound = new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg");
}
