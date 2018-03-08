using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;

[System.Serializable]
public class DataController : MonoBehaviour {
    static SocketIOComponent socket;
    public RoundData[] allRoundData;

    private bool hasBeenSent = true;

	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("retrieve data", OnRetrieveData);
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("MenuScreen");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public RoundData GetCurrentRoundData(int currentRound)
    {
        return allRoundData[currentRound];
    }

    // SocketIO
    void OnConnected(SocketIOEvent e)
    {
        Debug.Log("We are Connected");
        ConnectionMade();
        SendGameData();
    }
    void ConnectionMade()
    {
        socket.Emit("connection made");
    }

    void OnRetrieveData(SocketIOEvent e)
    {
        
    }

    void SendGameData()
    {
        if (hasBeenSent)
        {
            string jsonObj = JsonUtility.ToJson(this);
            socket.Emit("senddata", new JSONObject(jsonObj));
            Debug.Log(jsonObj);
            hasBeenSent = false;
        }
    }
}
