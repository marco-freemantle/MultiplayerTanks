using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    // Checks if the coin's position has changed and shows the coin if it has.
    private void Update()
    {
        if(previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }

    public override int Collect()
    {
        // If the game is not running on the server, hide the coin and return 0.
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected) return 0;

        alreadyCollected = true;

        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
