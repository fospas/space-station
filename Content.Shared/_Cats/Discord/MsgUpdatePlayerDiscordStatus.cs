using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Cats.Discord;

public sealed class MsgUpdatePlayerDiscordStatus : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public DiscordSponsorInfo? Info { get; set; }

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        if (buffer.ReadBoolean())
        {
            buffer.ReadPadBits();
            var length = buffer.ReadVariableInt32();
            using var stream = new MemoryStream();
            buffer.ReadAlignedMemory(stream, length);
            serializer.DeserializeDirect<DiscordSponsorInfo>(stream, out var info);

            Info = info;
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Info is not null);
        buffer.WritePadBits();

        if (Info is null)
        {
            return;
        }

        var stream = new MemoryStream();
        serializer.SerializeDirect(stream, Info);
        buffer.WriteVariableInt32((int) stream.Length);
        buffer.Write(stream.AsSpan());
    }
}
