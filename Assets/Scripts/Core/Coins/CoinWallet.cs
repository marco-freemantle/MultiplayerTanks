using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>(0);
    [SerializeField] private AudioSource coinSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Coin coin)) return;

        int coinValue = coin.Collect();

        if (IsLocalPlayer)
        {
            coinSound.Play();
        }

        if (!IsServer) return;

        TotalCoins.Value += coinValue;
    }
}
