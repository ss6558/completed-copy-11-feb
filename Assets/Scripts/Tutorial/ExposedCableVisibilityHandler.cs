using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposedCableVisibilityHandler : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;  // Reference to an AudioSource on a different, always-active GameObject
    public AudioClip pickupClip;     // Sound to play when picking up/hiding cable

    public void OnInteract()
    {
        // 1) Play the sound effect from the separate AudioSource
        if (audioSource != null && pickupClip != null)
        {
            audioSource.PlayOneShot(pickupClip);
        }

        // 2) Deactivate this cable object
        gameObject.SetActive(false);
    }
}
