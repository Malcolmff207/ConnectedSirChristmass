using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerScore: INetworkSerializable, IEquatable<PlayerScore>
{
    public ulong clientId;
    public FixedString64Bytes playerName;
    public int score;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref score);
    }
    public bool Equals(PlayerScore other)
    {
        return clientId == other.clientId && score == other.score && playerName == other.playerName;
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerScore other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(clientId, score);
    }
}
