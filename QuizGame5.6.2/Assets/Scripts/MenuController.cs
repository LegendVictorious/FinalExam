using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System.IO;

public class MenuController : MonoBehaviour {
    string scoreDataFilePath = "/StreamingAssets/scoreData.json";
    private RoundData[] allRoundDataArray;
    public SocketIOComponent socket;
    private bool hasEmitted = false;
    string filePath;

    void Start()
    {
        filePath = Application.dataPath + scoreDataFilePath;
        socket.On("open", OnConnected);
        socket.On("score data retrieved", OnScoreDataRetrieved);
    }

    // Use this for initialization
    public void StartGame () {
        SceneManager.LoadScene("Game");
	}

    public struct StringWrapper { public string wrappedString; }
    void OnConnected(SocketIOEvent e)
    {
        if (!hasEmitted)
        {
            DoConnectionTasks();
        }
    }

    void DoConnectionTasks()
    {
        
        StringWrapper wrappedPath = new StringWrapper();
        wrappedPath.wrappedString = filePath;
        string jsonObj = JsonUtility.ToJson(wrappedPath);
        socket.Emit("retrieve score data", new JSONObject(jsonObj));
        hasEmitted = true;
    }

    void OnScoreDataRetrieved(SocketIOEvent e)
    {
        //LoadScoreDataFromFile();
    }

    void LoadScoreDataFromFile()
    {
        string filePath = Application.dataPath + scoreDataFilePath;

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            AllRoundData allRoundData = JsonUtility.FromJson<AllRoundData>(data);
            allRoundDataArray = allRoundData.roundDataArray;
        }
    }
}
