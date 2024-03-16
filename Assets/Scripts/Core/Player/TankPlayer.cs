using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    override public void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            playerName.Value = userData.userName;
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
}
