#if UNITY_EDITOR
using UnityEditor;  // Needed for EditorApplication.isPlaying
#endif

using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    [Header("UI References")]
    public Button exitGameButton; // Assign in Inspector

    private void Start()
    {
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameClicked);
        }
        else
        {
            Debug.LogWarning("ExitGameButton: 'exitGameButton' is not assigned in the Inspector!");
        }
    }

    public void OnExitGameClicked()
    {
        // This will stop the application if you're in a built application.
        // In the editor, we also stop Play mode with #if UNITY_EDITOR.
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; 
#else
        Application.Quit();
#endif
        Debug.Log("Exit Game button clicked. Application quitting...");
    }
}
