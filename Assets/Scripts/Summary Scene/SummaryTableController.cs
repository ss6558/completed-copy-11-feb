using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class SummaryTableController : MonoBehaviour
{
    [Header("Up to 10 rows for Username, Score, Time")]
    public TextMeshProUGUI[] usernameTexts;   // 10 Text objects for usernames
    public TextMeshProUGUI[] scoreTexts;      // 10 Text objects for scores
    public TextMeshProUGUI[] finalTimeTexts;  // 10 Text objects for times

    private void Start()
    {
        // 1) Load data from CSV
        List<PlayerData> players = LoadAllPlayersFromCSV();

        // 2) Find current user + that user’s group
        string user = TraineeRegister.currentUsername;
        string userGroup = FindGroupForUser(user, players);

        // 3) Filter players to only those with matching group
        players = players.FindAll(p => p.groupNumber == userGroup);

        // (Optional) Sort by finalTime ascending:
        // players.Sort((a,b) => a.finalTime.CompareTo(b.finalTime));

        // 4) Show up to 10
        for (int i = 0; i < 10; i++)
        {
            if (i < players.Count)
            {
                usernameTexts[i].text = players[i].username;
                scoreTexts[i].text = $"{players[i].correctedCount}/{players[i].testedCount}";
                finalTimeTexts[i].text = FormatTime(players[i].finalTime);
            }
            else
            {
                usernameTexts[i].text = "";
                scoreTexts[i].text = "";
                finalTimeTexts[i].text = "";
            }
        }
    }

    private string FindGroupForUser(string user, List<PlayerData> allPlayers)
    {
        // look for a PlayerData entry matching username
        var found = allPlayers.Find(p => p.username == user);
        if (found.username == user) // means found is valid
        {
            return found.groupNumber;
        }
        return "";
    }

    /// <summary>
    /// Reads the CSV lines. Each row is:
    ///   username, groupNumber, finalTime, ..., correctedCount, testedCount
    ///   e.g. "Alice,G1B1,120,Corrected,NotCorrected,3,4"
    /// </summary>
    private List<PlayerData> LoadAllPlayersFromCSV()
    {
        List<PlayerData> result = new List<PlayerData>();
        string filePath = Application.dataPath + "/TraineeAccounts.csv";

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("TraineeAccounts.csv not found. No data to display.");
            return result;
        }

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            // skip header if desired
            if (i == 0 && line.StartsWith("username,"))
                continue;

            string[] cols = line.Split(',');
            if (cols.Length < 2) // we at least expect user + group
                continue;

            string username = cols[0].Trim();
            string group = cols[1].Trim();

            // col[2] might be finalTime if it exists
            float timeVal = 0f;
            int correctedVal = 0;
            int testedVal = 0;

            if (cols.Length >= 4)
            {
                float.TryParse(cols[2], out timeVal);

                // last 2 are correctedCount, testedCount if they exist
                if (cols.Length >= 6)
                {
                    // columns[cols.Length-2], columns[cols.Length-1]
                    string correctedStr = cols[cols.Length - 2];
                    string testedStr = cols[cols.Length - 1];
                    int.TryParse(correctedStr, out correctedVal);
                    int.TryParse(testedStr, out testedVal);
                }
            }

            PlayerData pd = new PlayerData()
            {
                username = username,
                groupNumber = group,
                finalTime = timeVal,
                correctedCount = correctedVal,
                testedCount = testedVal
            };

            result.Add(pd);
        }

        return result;
    }

    // Convert seconds to MM:SS
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

public struct PlayerData
{
    public string username;
    public string groupNumber;  // new field
    public float finalTime;
    public int correctedCount;
    public int testedCount;
}
