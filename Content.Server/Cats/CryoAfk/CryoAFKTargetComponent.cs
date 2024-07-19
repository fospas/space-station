namespace Content.Server.Cats.CryoTeleport;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CryoAFKTargetComponent : Component
{
    [DataField]
    public EntityUid? Station;

    [DataField]
    public TimeSpan? ExitTime;
}