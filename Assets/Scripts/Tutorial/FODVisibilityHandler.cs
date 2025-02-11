using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FODVisibilityHandler : MonoBehaviour
{
    [Header("Visual Effect")]
    public GameObject clearEffectPrefab; // Particle effect prefab (e.g. "DustPoof")

    [Header("Audio")]
    public AudioSource audioSource;   // Reference to an AudioSource on a different, always-active GameObject
    public AudioClip pickupClip;      // Sound to play when picking up this FOD

    public void OnInteract()
    {
        // 1) Play sound from the separate AudioSource
        if (audioSource != null && pickupClip != null)
        {
            audioSource.PlayOneShot(pickupClip);
        }

        // 2) Spawn particle effect at this object's position
        if (clearEffectPrefab != null)
        {
            Instantiate(clearEffectPrefab, transform.position, Quaternion.identity);
        }

        // 3) Deactivate the FOD object
        gameObject.SetActive(false);
    }
}
