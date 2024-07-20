using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared._Cats.DiscordLink;

namespace Content.Server._Cats.Discord
{
    public sealed class DiscordLinkEui : BaseEui
    {
        private string? _linkKey;

        public DiscordLinkEui()
        {
            IoCManager.InjectDependencies(this);
        }

        public override EuiStateBase GetNewState()
        {
            return new DiscordLinkEuiState(_linkKey);
        }

        public void SetLinkKey(string? linkKey)
        {
            _linkKey = linkKey;

            StateDirty();
        }
    }
}
