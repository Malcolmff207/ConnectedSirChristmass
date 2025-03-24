using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class PresentManager : NetworkBehaviour
{
    public static PresentManager Instance { get; private set; } //Singleton Setup - To make sure we always have one instance of PresentManager

    [SerializeField] private GameObject presentPrefab;

    private List<Transform> _spawnPoints; //Stores the spawn points
    private GameObject _currentPresent;
    private int _lastSpawnIndex = -1; //Store the last used spawn index

    public NetworkList<PlayerScore> playerScores;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPresentAtRandomServerRpc();
        }
        //Subscribe to changes in the list
        playerScores.OnListChanged += OnPlayerScoresChanged;
    }

    public override void OnNetworkDespawn()
    {
        //Unsubscribe to changes in the list
        playerScores.OnListChanged -= OnPlayerScoresChanged;
    }

    private void OnPlayerScoresChanged(NetworkListEvent<PlayerScore> changeEvent)
    {
        //List of clients who will receive the Client RPC
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
               TargetClientIds = new ulong[]{changeEvent.Value.clientId}
            }
        };
        
        //TODO
        //ADD NAME to the debug output
        switch (changeEvent.Type)
        {
            case NetworkListEvent<PlayerScore>.EventType.Add:
                Debug.Log($"Player added: ID={changeEvent.Value.clientId}, PlayerName= {changeEvent.Value.playerName}, Score={changeEvent.Value.score}");
                DisplayUpdatedScoreClientRpc(changeEvent.Value.score, clientRpcParams);
                break;
            case NetworkListEvent<PlayerScore>.EventType.Value:
                Debug.Log($"Player score updated: ID={changeEvent.Value.clientId}, PlayerName= {changeEvent.Value.playerName},Score={changeEvent.Value.score}");
                DisplayUpdatedScoreClientRpc(changeEvent.Value.score, clientRpcParams);
                break;
            case NetworkListEvent<PlayerScore>.EventType.Remove:
                Debug.Log($"Player removed: ID={changeEvent.Value.clientId}");
                break;  
            case NetworkListEvent<PlayerScore>.EventType.Clear:
                Debug.Log($"Players scores are reset.");
                break; 
        }
    }

    //Only going to be executed on the client whose score has changed
    [ClientRpc]
    private void DisplayUpdatedScoreClientRpc(int score, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"Your updated score is {score}!");
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerInfoServerRpc(ulong clientId, int points, FixedString64Bytes playerName = default)
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i].clientId == clientId)
            {
                //update the score of an existing player
                playerScores[i] = new PlayerScore { clientId = clientId,  playerName = playerScores[i].playerName,score = playerScores[i].score + points };
                return;
            }
        }
        //if the playerId is not found in the list, we add him
        playerScores.Add(new PlayerScore{clientId = clientId, playerName = playerName, score= points});
    }

    private void Awake()
    {
        //Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerScores = new NetworkList<PlayerScore>();
        //Loading all the spawn points from the scene to the list
        _spawnPoints = new List<Transform>();
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            _spawnPoints.Add(spawnPoint.transform);
        }
    }
    
    //This method is going to only execute on the Server
    [ServerRpc]
    private void SpawnPresentAtRandomServerRpc()
    {
        if (_spawnPoints.Count == 0) return;

        //keep searching for a spawn point that is not the last spawn point
        int index;
        do
        {
            index = UnityEngine.Random.Range(0, _spawnPoints.Count);
        } while (index == _lastSpawnIndex && _spawnPoints.Count > 1);
        
        //set the spawn position and update last spawn index
        Transform spawnPoint = _spawnPoints[index];
        _lastSpawnIndex = index;
        
        //Spawn the prefab on the server
        _currentPresent = Instantiate(presentPrefab, spawnPoint.position, Quaternion.identity);
        
        //Spawn the prefab across the network
        NetworkObject networkObject = _currentPresent.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    public void PresentCollected()
    {
        if (_currentPresent != null)
        {
            Destroy(_currentPresent);
        }
        SpawnPresentAtRandomServerRpc();
    }
}
