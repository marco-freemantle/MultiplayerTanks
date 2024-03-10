using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    
    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Hit");
        if(other.attachedRigidbody == null) return;
        print("Has rigid body");

        if(other.attachedRigidbody.TryGetComponent(out NetworkObject netObj))
        {
            if (netObj.OwnerClientId == ownerClientId) return;
        }
        print("Not owner");
        if(other.attachedRigidbody.TryGetComponent(out Health health))
        {
            print("Has health");
            health.TakeDamage(damage);
        }
    }
}
