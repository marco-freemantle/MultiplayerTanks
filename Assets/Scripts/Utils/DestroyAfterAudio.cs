using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
