using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PresentCollect : NetworkBehaviour
{
    private bool isCollected = false;
    private bool hasLanded = false;
    private Array _presentTypes;
    private enum PresentType
    {
        SMALL = 5,
        MEDIUM = 10,
        LARGE = 20,
        XL = 30
    };

    private void Start()
    {
        _presentTypes = Enum.GetValues(typeof(PresentType));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground")) hasLanded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasLanded || isCollected || !other.CompareTag("Player")) return;
        ulong clientId = other.GetComponent<NetworkObject>().OwnerClientId;
        CollectPresentServerRpc(clientId);
    }

    [ServerRpc]
    private void CollectPresentServerRpc(ulong clientId)
    {
        if (isCollected) return; //Server side check to prevent double collection
        isCollected = true;
        
        //Award Points
        
        int randomPresent = UnityEngine.Random.Range(0, _presentTypes.Length);
        int points = (int) _presentTypes.GetValue(randomPresent);
        PresentManager.Instance.UpdatePlayerInfoServerRpc(clientId,points);
        
        //Spawn the next present
        PresentManager.Instance.PresentCollected();
        
        //Apply a boon to the player
    }
}
