namespace Content.Server.Cats.CryoTeleport;

[RegisterComponent]
public sealed partial class CryoAFKTargetComponent : Component
{
    [DataField]
    public EntityUid? Station;

    [DataField]
    public TimeSpan? ExitTime;
}
