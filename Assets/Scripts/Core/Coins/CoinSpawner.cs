using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;

    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;

    // Buffer to hold collider references.
    private Collider2D[] coinBuffer = new Collider2D[1];

    // Radius of the coin.
    private float coinRadius;

    // This method is called when the object is spawned in the network.
    public override void OnNetworkSpawn()
    {
        // If not running on the server, return.
        if (!IsServer) return;

        // Get the radius of the coin.
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        // Spawn the maximum number of coins.
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    // Spawns a coin at a random position within the specified range.
    private void SpawnCoin()
    {
        // Instantiate a new coin instance at a random spawn point.
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);

        // Set the value of the coin.
        coinInstance.SetValue(coinValue);

        // Spawn the coin in the network.
        coinInstance.GetComponent<NetworkObject>().Spawn();

        // Subscribe to the OnCollected event of the coin instance.
        coinInstance.OnCollected += HandleCoinCollected;
    }

    // Resets the position and state of the coin when it is collected.
    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    // Gets a random spawn point for the coin.
    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while (true)
        {
            // Generate random coordinates within the specified ranges.
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            // Check if there are any colliders in the spawn area.
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            // If no colliders are found, return the spawn point.
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
