using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEInteractionHandler : MonoBehaviour
{
    public GameObject initialCanvas;
    public GameObject correctCanvas;
    public GameObject incorrectCanvas;

    public void OnInteract()
    {
        // Hide the other canvases first (so they don't stay visible if previously selected)
        if (correctCanvas != null)
            correctCanvas.SetActive(false);

        if (incorrectCanvas != null)
            incorrectCanvas.SetActive(false);

        // Now show the initialCanvas
        if (initialCanvas != null)
            initialCanvas.SetActive(true);
    }

    public void OnYesSelected()
    {
        if (initialCanvas != null)
            initialCanvas.SetActive(false);

        if (correctCanvas != null)
            correctCanvas.SetActive(true);
    }

    public void OnNoSelected()
    {
        if (initialCanvas != null)
            initialCanvas.SetActive(false);

        if (incorrectCanvas != null)
            incorrectCanvas.SetActive(true);
    }
}
