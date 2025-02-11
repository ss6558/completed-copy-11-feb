using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SummarySceneHandler : MonoBehaviour
{
    public TextMeshProUGUI antennaErrorText;
    public TextMeshProUGUI exposedCableErrorText;
    public TextMeshProUGUI rightCornerboardErrorText;
    public TextMeshProUGUI leftCornerboardErrorText;
    public TextMeshProUGUI leftLadderErrorText;
    public TextMeshProUGUI rightLadderErrorText;
    public TextMeshProUGUI groundingRodErrorText;
    public TextMeshProUGUI fireExtinguisherErrorText;
    public TextMeshProUGUI fodErrorText;
    public TextMeshProUGUI ppeErrorText;
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        DisplayAntennaText();
        DisplayExposedCableText();
        DisplayRightCornerboardText();
        DisplayLeftCornerboardText();
        DisplayLeftLadderText();
        DisplayRightLadderText();
        DisplayGroundingRodText();
        DisplayFireExtinguisherText();
        DisplayFODsText();
        DisplayPPEText();
        DisplayTime();

        SaveSummaryData();
    }

    private void DisplayErrorStatus(string errorKey, TextMeshProUGUI textMesh)
    {
        if (PersistentDataStore.errorStatuses.TryGetValue(errorKey, out var data))
        {
            string message = $"{errorKey}: {data.status}";
            Color color = GetStatusColor(data.status);
            SetText(textMesh, message, color);
        }
        else
        {
            SetText(textMesh, $"{errorKey}: Status Unknown", Color.black);
        }
    }

    private Color GetStatusColor(ErrorStatus status)
    {
        switch (status)
        {
            case ErrorStatus.Corrected: return Color.green;
            case ErrorStatus.NotCorrected: return Color.red;
            case ErrorStatus.NotTested: return Color.gray;
            default: return Color.black;
        }
    }

    private void DisplayAntennaText() => DisplayErrorStatus("Antenna Error", antennaErrorText);
    private void DisplayExposedCableText() => DisplayErrorStatus("Cable Error", exposedCableErrorText);
    private void DisplayRightCornerboardText() => DisplayErrorStatus("Right Cornerboard Error", rightCornerboardErrorText);
    private void DisplayLeftCornerboardText() => DisplayErrorStatus("Left Cornerboard Error", leftCornerboardErrorText);
    private void DisplayLeftLadderText() => DisplayErrorStatus("Left Ladder Error", leftLadderErrorText);
    private void DisplayRightLadderText() => DisplayErrorStatus("Right Ladder Error", rightLadderErrorText);
    private void DisplayGroundingRodText() => DisplayErrorStatus("Grounding Rod Error", groundingRodErrorText);
    private void DisplayFireExtinguisherText() => DisplayErrorStatus("Fire Extinguisher Error", fireExtinguisherErrorText);
    private void DisplayFODsText() => DisplayErrorStatus("FOD Error", fodErrorText);
    private void DisplayPPEText() => DisplayErrorStatus("PPE Error", ppeErrorText);

    private void DisplayTime()
    {
        float finalTime = PersistentDataStore.assessmentTime;
        int mins = Mathf.FloorToInt(finalTime / 60f);
        int secs = Mathf.FloorToInt(finalTime % 60f);
        finalTimeText.text = $"Final time: {mins:00}:{secs:00}";
    }

    private void SetText(TextMeshProUGUI textComponent, string message, Color color)
    {
        textComponent.text = message;
        textComponent.color = color;
    }

    // ---------------------------------
    // Save summary to CSV
    // ---------------------------------
    private void SaveSummaryData()
    {
        string user = TraineeRegister.currentUsername;
        if (string.IsNullOrEmpty(user))
        {
            Debug.LogWarning("No current user found!");
            return;
        }

        float finalTime = PersistentDataStore.assessmentTime;

        // gather existing statuses
        string antennaStatus = GetErrorStatusString("Antenna Error");
        string cableStatus = GetErrorStatusString("Cable Error");
        string rightCornerStatus = GetErrorStatusString("Right Cornerboard Error");
        string leftCornerStatus = GetErrorStatusString("Left Cornerboard Error");
        string leftLadderStatus = GetErrorStatusString("Left Ladder Error");
        string rightLadderStatus = GetErrorStatusString("Right Ladder Error");
        string groundingRodStatus = GetErrorStatusString("Grounding Rod Error");
        string fireExtinguisherStatus = GetErrorStatusString("Fire Extinguisher Error");
        string fodStatus = GetErrorStatusString("FOD Error");

        // new ppeStatus
        string ppeStatus = GetErrorStatusString("PPE Error");

        // compute corrected/tested count as before, now include ppeStatus
        int correctedCount = 0;
        int testedCount = 0;
        string[] allStatuses = {
        antennaStatus,
        cableStatus,
        rightCornerStatus,
        leftCornerStatus,
        leftLadderStatus,
        rightLadderStatus,
        groundingRodStatus,
        fireExtinguisherStatus,
        fodStatus,
        ppeStatus
    };

        foreach (string s in allStatuses)
        {
            if (s == ErrorStatus.Corrected.ToString())
            {
                correctedCount++;
                testedCount++;
            }
            else if (s == ErrorStatus.NotCorrected.ToString())
            {
                testedCount++;
            }
        }

        // set score text if desired
        if (scoreText != null)
        {
            scoreText.text = $"Score: {correctedCount}/{testedCount}";
        }

        // read groupNumber from CSV or memory
        string groupNumber = GetGroupNumberFromCSV(user);

        // Now call UpdateTraineeRow with the new ppeStatus param
        TraineeRegister.UpdateTraineeRow(
            user,
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
            ppeStatus,  // new param
            correctedCount,
            testedCount
        );
    }

    private string GetErrorStatusString(string key)
    {
        if (PersistentDataStore.errorStatuses.TryGetValue(key, out var data))
        {
            return data.status.ToString();
        }
        return "Unknown";
    }

    // Basic example: read CSV to find user row, return groupNumber from col [1]
    private string GetGroupNumberFromCSV(string user)
    {
        string csvFile = Application.dataPath + "/TraineeAccounts.csv";
        if (!File.Exists(csvFile))
        {
            return "";
        }

        string[] lines = File.ReadAllLines(csvFile);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            string[] cols = line.Split(',');
            if (cols.Length > 1)
            {
                // col[0] = username, col[1] = groupNumber
                if (cols[0].Trim().Equals(user, System.StringComparison.OrdinalIgnoreCase))
                {
                    return cols[1].Trim();
                }
            }
        }
        return "";
    }
}
