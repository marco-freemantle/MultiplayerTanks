using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Abstract class means that it can't be instantiated, but it can be inherited from
public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Protcted means that it can only be accessed by this class and its children
    protected int coinValue = 10;
    protected bool alreadyCollected;

    // Abstract method means that it has to be implemented by the children
    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }    

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }
}
