using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameButtonHandler : MonoBehaviour
{
    public Button newGameButton; // Assign this button in the Inspector
    public string loginSceneName = "Login"; // The name of your login scene

    private void Start()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }
        else
        {
            Debug.LogWarning("NewGameButton is not assigned in the Inspector!");
        }
    }

    public void OnNewGameClicked()
    {
        // Optional: If you need to reset any persistent data, do it here
        // e.g. PersistentDataStore.errorStatuses.Clear();

        // Load the login scene to start a new game
        SceneManager.LoadScene("Login");
    }
}
