using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        //Subscribe to ChatBox commands
        ChatBox.Instance.OnCommandSent.AddListener(CommandReceived);
        ChatBox.Instance.OnMessageSent.AddListener(ChatMessageSent);
    }

    public override void OnNetworkDespawn()
    {
        //Unsubscribe to ChatBox commands
        ChatBox.Instance.OnCommandSent.RemoveListener(CommandReceived);
        ChatBox.Instance.OnMessageSent.RemoveListener(ChatMessageSent);
    }

    //When the player inputs a message in chat - we have to send it to the rest of the players in game
    private void ChatMessageSent(string message)
    {
        AddMessageToChatboxServerRPC($"{GetPlayerName()}: {message}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddMessageToChatboxServerRPC(string message)
    {
        DisplayMessageClientRPC(message);
    }
    
    [ClientRpc]
    private void DisplayMessageClientRPC(string message)
    {
        ChatBox.Instance.CreateChatMessage(message);
    }

    private void CommandReceived(string command)
    {
        //Defining a the playerName command
        if (command.StartsWith("::playerName "))
        {
            string playerName = command.Substring(13);
            ChatBox.Instance.CreateChatMessage($"Hello {playerName}!");
            PresentManager.Instance.UpdatePlayerInfoServerRpc(NetworkManager.Singleton.LocalClientId,
                0, playerName);
        }
    }

    private string GetPlayerName()
    {
        for (int i = 0; i <= PresentManager.Instance.playerScores.Count; i++)
            {
                if (PresentManager.Instance.playerScores[i].clientId == NetworkManager.Singleton.LocalClientId)
                {
                    return PresentManager.Instance.playerScores[i].playerName.ToString();
                }
            }
        return "player";
    }
}
// public struct PlayerData : INetworkSerializable
    // {
    //     public string playerName;
    //     public int score;
    //     public bool isActive;
    //     public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    //     {
    //         serializer.SerializeValue(ref playerName);
    //         serializer.SerializeValue(ref score);
    //         serializer.SerializeValue(ref isActive);
    //     }
    // }
    //
    // private NetworkVariable<PlayerData> _playerData = new NetworkVariable<PlayerData>(
    //     new PlayerData{playerName = "Tom", score = 15, isActive = true},
    //     NetworkVariableReadPermission.Everyone, 
    //     NetworkVariableWritePermission.Owner);
    //
    // private NetworkVariable<int> _playerRoll = 
    //     new NetworkVariable<int>(0, NetworkVariableReadPermission.Owner,
    //     NetworkVariableWritePermission.Owner);
    //
    // private NetworkVariable<FixedString32Bytes> _greeting = new NetworkVariable<FixedString32Bytes>
    // ("Greetings",NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    //
    // public override void OnNetworkSpawn()
    // {
    //     //Subscribe to changes in _playerRoll
    //     _playerRoll.OnValueChanged += PlayerRollChanged;
    //     //Subscribe to changes in _greeting
    //     _greeting.OnValueChanged += PlayerGreetingChanged;
    //     
    //     Debug.Log(_playerData.Value.playerName);
    //     Debug.Log(_playerData.Value.score);
    //     Debug.Log(_playerData.Value.isActive);
    // }
    //
    //
    // public override void OnNetworkDespawn()
    // {
    //     //Unsubscribe to changes in _playerRoll
    //     _playerRoll.OnValueChanged -= PlayerRollChanged;
    //     //Unsubscribe to changes in _greeting
    //     _greeting.OnValueChanged -= PlayerGreetingChanged;
    // }
    //
    // private void PlayerRollChanged(int prevRoll, int newRoll)
    // {
    //     Debug.Log($"The roll value of {OwnerClientId} has changed from {prevRoll} to {newRoll}");    
    // }
    //
    // private void PlayerGreetingChanged(FixedString32Bytes prevGreet, FixedString32Bytes newGreet)
    // {
    //     Debug.Log($"{newGreet}");    
    // }
    //
    // private void Update()
    // {
    //     if (!IsOwner) return;
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         _playerRoll.Value = Random.Range(0, 100);
    //     }
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         _greeting.Value = "Happy Halloween!";
    //     }
    // }