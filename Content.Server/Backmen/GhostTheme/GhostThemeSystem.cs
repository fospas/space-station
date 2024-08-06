using System.Linq;
using Content.Corvax.Interfaces.Server;
using Content.Corvax.Interfaces.Shared;
using Content.Server.GameTicking;
using Content.Shared.Backmen.GhostTheme;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared._Cats.Events;
using Robust.Server.Configuration;
using Robust.Server.Console;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Content.Server.SharedContent;

public sealed class GhostThemeSystem : EntitySystem
{
    [Dependency] private readonly ISharedSponsorsManager _sponsorsMgr = default!; // Corvax-Sponsors
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;
    [Dependency] private readonly IServerNetConfigurationManager _netConfigManager = default!;
    [Dependency] private readonly IServerConsoleHost _console = default!;

    private readonly Dictionary<string, bool> _respawnUsedDictionary = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GhostComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnGameRunLevelChanged);
        SubscribeNetworkEvent<RespawnRequestEvent>(OnGhostRespawnRequest);
    }

    private void OnGameRunLevelChanged(GameRunLevelChangedEvent ev)
    {
        if (ev.New == GameRunLevel.PreRoundLobby)
        {
            _respawnUsedDictionary.Clear();
        }
    }

    private void OnGhostRespawnRequest(RespawnRequestEvent msg, EntitySessionEventArgs args)
    {
        if (args.SenderSession.AttachedEntity == null ||
            !TryComp<GhostComponent>(args.SenderSession.AttachedEntity.Value, out var ghostComponent))
            return;
        if (_respawnUsedDictionary.ContainsKey(args.SenderSession.UserId.UserId.ToString()))
        {
            return;
        }

        if (ghostComponent.TimeOfDeath.CompareTo(ghostComponent.TimeOfDeath.Add(TimeSpan.FromMinutes(15))) is -1 or 0)
            _console.ExecuteCommand($"respawn {args.SenderSession.Name}");
        _respawnUsedDictionary.Add(args.SenderSession.UserId.UserId.ToString(), true);
    }

    private void OnPlayerAttached(EntityUid uid, GhostComponent component, PlayerAttachedEvent args)
    {
        var prefGhost = _netConfigManager.GetClientCVar(args.Player.Channel, Shared.Backmen.CCVar.CCVars.SponsorsSelectedGhost);
        {
#if DEBUG
            if (!_sponsorsMgr.TryGetServerPrototypes(args.Player.UserId, out var items))
            {
                items = new List<string>();
                items.Add("tier1");
                items.Add("tier2");
                items.Add("tier01");
                items.Add("tier02");
                items.Add("tier03");
                items.Add("tier04");
                items.Add("tier05");
                items.Add("tier06");
                items.Add("tier07");
                items.Add("tier08");
            }
            if (!items.Contains(prefGhost))
            {
                prefGhost = "";
            }
#else
            if (!_sponsorsMgr.TryGetServerPrototypes(args.Player.UserId, out var items) || !items.Contains(prefGhost))
            {
                prefGhost = "";
            }
#endif
        }

        GhostThemePrototype? ghostThemePrototype = null;
        if (string.IsNullOrEmpty(prefGhost) || !_prototypeManager.TryIndex<GhostThemePrototype>(prefGhost, out ghostThemePrototype))
        {
            if (!_sponsorsMgr.TryGetGhostTheme(args.Player.UserId, out var ghostTheme) ||
                !_prototypeManager.TryIndex(ghostTheme, out ghostThemePrototype)
               )
            {
                return;
            }
        }

        foreach (var entry in ghostThemePrototype.Components.Values)
        {
            var comp = (Component) _serialization.CreateCopy(entry.Component, notNullableOverride: true);
            comp.Owner = uid;
            EntityManager.AddComponent(uid, comp);
        }

        EnsureComp<GhostThemeComponent>(uid).GhostTheme = ghostThemePrototype.ID;
    }
}
