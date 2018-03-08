using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DataController : MonoBehaviour {

    public RoundData[] allRoundData;

	// Use this for initialization
	void Start () {
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
}
