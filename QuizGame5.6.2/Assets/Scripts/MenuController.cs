using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System.IO;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    private string scoreDataFilePath = "/StreamingAssets/scoreData.json";
    public SocketIOComponent socket;
    private bool hasConnected = false;
    private bool hasEmitted = false;
    private bool hasLoadedScores = false;
    private string filePath;
    private ScoreData[] scoreDataArray;

    public GameObject mainMenuPanel;
    public GameObject highScorePanel;
    public Text highScoreText;

    public struct StringWrapper { public string wrappedString; }
    public struct ScoreDataArrayWrapper { public ScoreData[] scoreDataArray; }

    void Start()
    {       
        socket.On("open", OnConnected);
    }

    void Update()
    {
        if (hasConnected && (hasEmitted == false))
        {
            filePath = Application.dataPath + scoreDataFilePath;
            StringWrapper wrappedPath = new StringWrapper();
            wrappedPath.wrappedString = filePath;
            string jsonObj = JsonUtility.ToJson(wrappedPath);
            System.Threading.Thread.Sleep(1000);
            socket.Emit("retrieve score data", new JSONObject(jsonObj));
            hasEmitted = true;
        }
    }

    void OnConnected(SocketIOEvent e)
    {
        hasConnected = true;
    }

    // Buttons
    public void StartGame () {
        SceneManager.LoadScene("Game");
	}  
    
    public void DisplayScoreScreen()
    {
        mainMenuPanel.SetActive(false);
        highScorePanel.SetActive(true);
        if (!hasLoadedScores)
        {
            LoadScoreDataFromFile();
        }
        DisplayScoreArray();
    }
    
    void DisplayScoreArray()
    {
        highScoreText.text = "";
        for (int i = 0; (i < 10) && (i < scoreDataArray.Length); i++)
        {
            highScoreText.text += scoreDataArray[i].playerInitials + " " + scoreDataArray[i].playerScore + "\n";
        }
    } 

    public void DisplayMainMenu()
    {
        highScorePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }    


    void LoadScoreDataFromFile()
    {
        string filePath = Application.dataPath + scoreDataFilePath;

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            ScoreDataArrayWrapper scoreDataWrapper = JsonUtility.FromJson<ScoreDataArrayWrapper>(data);
            scoreDataArray = scoreDataWrapper.scoreDataArray;
            System.Array.Sort(scoreDataArray, delegate (ScoreData score1, ScoreData score2) {
                return -score1.playerScore.CompareTo(score2.playerScore);
            });
        }
    }
}
