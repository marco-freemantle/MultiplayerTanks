using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource movementSound;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turningRate = 30f;

    private Vector2 previousMovementInput;

    private bool isMovementSoundPlaying = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

    private void Update()
    {
        if(!IsOwner) return;

        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);

        if (rb.velocity.magnitude > 0.1f && !isMovementSoundPlaying)
        {
            movementSound.Play();
            isMovementSoundPlaying = true;
        }
        else if (rb.velocity.magnitude < 0.1f && isMovementSoundPlaying)
        {
            movementSound.Stop();
            isMovementSoundPlaying = false;
        }
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        rb.velocity = (Vector2)bodyTransform.up * previousMovementInput.y * moveSpeed;
    }
}
