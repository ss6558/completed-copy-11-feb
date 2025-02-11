using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPEAssessmentHandler : MonoBehaviour
{
    [Header("Helmet References")]
    public GameObject helmetObject;     // The 3D helmet GameObject
    public AudioSource helmetAudioSource;
    public AudioClip helmetAudioClip;   // "Safety Helmet Donned"

    [Header("Gloves References")]
    public GameObject glovesObject;     // The 3D gloves GameObject
    public GameObject leftHandModel;    // VR left hand GameObject
    public GameObject rightHandModel;   // VR right hand GameObject
    public Texture glovesTexture;       // The texture to apply to each hand's existing material
    public AudioSource glovesAudioSource;
    public AudioClip glovesAudioClip;   // If you have a "Gloves On" sound

    [Header("PPE Error Key")]
    public string ppeErrorKey = "PPE Error";

    private bool isHelmetDonned = false;
    private bool areGlovesOn = false;

    private void Start()
    {
        // Ensure the PPE error key exists in PersistentDataStore
        if (!PersistentDataStore.errorStatuses.ContainsKey(ppeErrorKey))
        {
            PersistentDataStore.errorStatuses[ppeErrorKey] = new ErrorStatusData();
        }

        // Initialize the status
        UpdatePPEStatus();
        Debug.Log($"Initial PPE status: {PersistentDataStore.errorStatuses[ppeErrorKey].status}");
    }

    /// <summary>
    /// Called when the player picks up or dons the helmet.
    /// Hides the helmet object, plays an audio clip, marks helmet as donned.
    /// </summary>
    public void InteractWithHelmet()
    {
        if (helmetObject == null)
        {
            Debug.LogWarning("Helmet object is not assigned!");
            return;
        }

        // Hide the helmet in the scene
        helmetObject.SetActive(false);
        isHelmetDonned = true;

        // Play helmet audio if assigned
        if (helmetAudioSource != null && helmetAudioClip != null)
        {
            helmetAudioSource.PlayOneShot(helmetAudioClip);
        }

        Debug.Log("Helmet donned!");
        UpdatePPEStatus();
    }

    /// <summary>
    /// Called when the player picks up or dons the gloves.
    /// Hides the gloves object, applies the gloves texture to the left/right hand models, 
    /// and marks gloves as on.
    /// </summary>
    public void InteractWithGloves()
    {
        if (glovesObject == null)
        {
            Debug.LogWarning("Gloves object is not assigned!");
            return;
        }

        // Hide the gloves object
        glovesObject.SetActive(false);
        areGlovesOn = true;

        // Play gloves audio if assigned
        if (glovesAudioSource != null && glovesAudioClip != null)
        {
            glovesAudioSource.PlayOneShot(glovesAudioClip);
        }

        // Apply the gloves texture to both hand models
        if (glovesTexture != null)
        {
            ApplyTextureToHand(leftHandModel, glovesTexture);
            ApplyTextureToHand(rightHandModel, glovesTexture);
        }

        Debug.Log("Gloves donned!");
        UpdatePPEStatus();
    }

    /// <summary>
    /// Applies the given texture to the main material of the specified hand model.
    /// </summary>
    private void ApplyTextureToHand(GameObject handModel, Texture tex)
    {
        if (handModel == null) return;

        // Attempt to get a Renderer (MeshRenderer or SkinnedMeshRenderer) from the hand
        var renderer = handModel.GetComponent<Renderer>();
        if (renderer == null)
        {
            // Possibly it's a child object that has the renderer
            renderer = handModel.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            // Make sure we get the instance of the material
            Material mat = renderer.material;
            mat.mainTexture = tex;
        }
        else
        {
            Debug.LogWarning($"No Renderer found on hand model: {handModel.name}");
        }
    }

    /// <summary>
    /// Checks if both helmet and gloves have been donned. If so, mark "PPE Error" = Corrected;
    /// otherwise, NotCorrected.
    /// </summary>
    private void UpdatePPEStatus()
    {
        var statusData = PersistentDataStore.errorStatuses[ppeErrorKey];

        // If both items are selected, status = Corrected; else NotCorrected
        if (isHelmetDonned && areGlovesOn)
        {
            statusData.status = ErrorStatus.Corrected;
        }
        else
        {
            statusData.status = ErrorStatus.NotCorrected;
        }

        Debug.Log($"PPE Error status updated: {statusData.status} (Helmet donned: {isHelmetDonned}, Gloves on: {areGlovesOn})");
    }
}
