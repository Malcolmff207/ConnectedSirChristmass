using UnityEngine;
using Unity.Netcode;
public class GameTimer : NetworkBehaviour
{
    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>(120f, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private void Update()
    {
        if (IsServer && timeRemaining.Value > 0)
        {
            timeRemaining.Value -= Time.deltaTime;
        }
    }
}
