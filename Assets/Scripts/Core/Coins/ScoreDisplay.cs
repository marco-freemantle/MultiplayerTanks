using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private TextMeshProUGUI scoreText;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        coinWallet.TotalCoins.OnValueChanged += HandleScoreChanged;
        HandleScoreChanged(0, coinWallet.TotalCoins.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;

        coinWallet.TotalCoins.OnValueChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int oldScore, int newScore)
    {
        scoreText.SetText("Score: " + newScore);
    }
}
