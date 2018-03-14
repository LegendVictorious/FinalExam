using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System.IO;

[System.Serializable]
public class DataController : MonoBehaviour {
    public AllRoundData allRoundData;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        RetrieveDataFromFile();
        SceneManager.LoadScene("MenuScreen");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public RoundData GetCurrentRoundData(int currentRound)
    {
        return allRoundData.roundDataArray[currentRound];
    }

    public void RetrieveDataFromFile()
    {
        string filePath = Application.dataPath + "/StreamingAssets/data.json";
        string data = File.ReadAllText(filePath);
        allRoundData = JsonUtility.FromJson<AllRoundData>(data);
    }
}
