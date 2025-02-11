#if UNITY_EDITOR
using UnityEditor;  // Needed for EditorApplication.isPlaying
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TraineeRegister : MonoBehaviour
{
    [Header("Login/Create UI")]
    public TMP_InputField usernameInputField;
    public TMP_InputField groupNumberInputField;  // new for group
    public Button enterButton;
    public Button createAccountButton;
    public Button quitButton;
    public TMP_Text messageText;

    public static string currentUsername;  // The user who logs in

    private string filePath;
    private List<string> knownUsers = new List<string>();

    void Start()
    {
        filePath = Application.dataPath + "/TraineeAccounts.csv";
        LoadUserData();

        enterButton.onClick.AddListener(HandleEnterButton);
        createAccountButton.onClick.AddListener(HandleCreateAccountButton);
        quitButton.onClick.AddListener(QuitApplication);
    }

    // --------------------------
    // LOGIN
    // --------------------------
    public void HandleEnterButton()
    {
        string username = usernameInputField.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            DisplayMessage("Please enter a username.");
            return;
        }

        // If user is found in CSV, set currentUsername and load MainMenu
        if (knownUsers.Contains(username))
        {
            currentUsername = username;
            DisplayMessage($"Welcome, {username}!");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            DisplayMessage("Account does not exist.");
        }
    }

    // --------------------------
    // CREATE ACCOUNT
    // --------------------------
    public void HandleCreateAccountButton()
    {
        string username = usernameInputField.text.Trim();
        string groupNumber = groupNumberInputField.text.Trim(); // new input

        if (string.IsNullOrEmpty(username))
        {
            DisplayMessage("Please enter a username.");
            return;
        }

        if (knownUsers.Contains(username))
        {
            DisplayMessage("Account already exists.");
            return;
        }

        // Write "username,groupNumber" to CSV
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine($"{username},{groupNumber}");
        }

        // Add to knownUsers so we can detect duplicates
        knownUsers.Add(username);

        DisplayMessage("Registration successful.");
    }

    // --------------------------
    // QUIT
    // --------------------------
    public void QuitApplication()
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

    // --------------------------
    // LOAD USERS
    // --------------------------
    private void LoadUserData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("User data file not found. Creating a new file.");
            File.Create(filePath).Dispose();
            return;
        }

        // Read each line "username,groupNumber"
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            string[] cols = line.Split(',');
            if (cols.Length > 0)
            {
                string user = cols[0].Trim();
                if (!string.IsNullOrEmpty(user) && !knownUsers.Contains(user))
                {
                    knownUsers.Add(user);
                }
            }
        }

        Debug.Log("User data loaded. Total users: " + knownUsers.Count);
    }

    // --------------------------
    // FEEDBACK
    // --------------------------
    private void DisplayMessage(string msg)
    {
        messageText.text = msg;
        StartCoroutine(ClearMessageAfterDelay(5f));
    }

    private System.Collections.IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = "";
    }

    // ----------------------------------------------------------------
    // STORING FINAL TIME/STATUSES
    //  Now includes "ppeStatus" param
    // ----------------------------------------------------------------
    public static void UpdateTraineeRow(
        string username,
        string groupNumber,
        float finalTime,
        string antennaStatus,
        string cableStatus,
        string rightCornerStatus,
        string leftCornerStatus,
        string leftLadderStatus,
        string rightLadderStatus,
        string groundingRodStatus,
        string fireExtinguisherStatus,
        string fodStatus,
        string ppeStatus,    // NEW PPE param
        int correctedCount,
        int testedCount
    )
    {
        string filePath = Application.dataPath + "/TraineeAccounts.csv";
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }

        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        bool foundUser = false;

        for (int i = 0; i < lines.Count; i++)
        {
            string[] cols = lines[i].Split(',');
            // col[0] is username
            if (cols.Length > 0 && cols[0].Trim().Equals(username, System.StringComparison.OrdinalIgnoreCase))
            {
                // Overwrite row
                lines[i] = GenerateCsvLine(
                    username,
                    groupNumber,
                    finalTime,
                    antennaStatus,
                    cableStatus,
                    rightCornerStatus,
                    leftCornerStatus,
                    leftLadderStatus,
                    rightLadderStatus,
                    groundingRodStatus,
                    fireExtinguisherStatus,
                    fodStatus,
                    ppeStatus,          // pass new param
                    correctedCount,
                    testedCount
                );
                foundUser = true;
                break;
            }
        }

        if (!foundUser)
        {
            // user not found, append
            lines.Add(
                GenerateCsvLine(
                    username,
                    groupNumber,
                    finalTime,
                    antennaStatus,
                    cableStatus,
                    rightCornerStatus,
                    leftCornerStatus,
                    leftLadderStatus,
                    rightLadderStatus,
                    groundingRodStatus,
                    fireExtinguisherStatus,
                    fodStatus,
                    ppeStatus,
                    correctedCount,
                    testedCount
                )
            );
        }

        File.WriteAllLines(filePath, lines.ToArray());
    }

    // columns:
    // 0: username
    // 1: groupNumber
    // 2: finalTime
    // 3: antenna
    // 4: cable
    // 5: rightCorner
    // 6: leftCorner
    // 7: leftLadder
    // 8: rightLadder
    // 9: groundingRod
    // 10: fireExtinguisher
    // 11: fod
    // 12: ppe (NEW)
    // 13: correctedCount
    // 14: testedCount
    private static string GenerateCsvLine(
        string username,
        string groupNumber,
        float finalTime,
        string antennaStatus,
        string cableStatus,
        string rightCornerStatus,
        string leftCornerStatus,
        string leftLadderStatus,
        string rightLadderStatus,
        string groundingRodStatus,
        string fireExtinguisherStatus,
        string fodStatus,
        string ppeStatus,  // new param
        int correctedCount,
        int testedCount
    )
    {
        return string.Join(",", new string[] {
            username,
            groupNumber,
            finalTime.ToString("F0"),
            antennaStatus,
            cableStatus,
            rightCornerStatus,
            leftCornerStatus,
            leftLadderStatus,
            rightLadderStatus,
            groundingRodStatus,
            fireExtinguisherStatus,
            fodStatus,
            ppeStatus,  // new column
            correctedCount.ToString(),
            testedCount.ToString()
        });
    }
}
