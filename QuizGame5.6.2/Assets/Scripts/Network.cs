using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Network : MonoBehaviour {
 	static SocketIOComponent socket;

	// Use this for initialization
	void Start () {
		socket = GetComponent<SocketIOComponent> ();
		socket.On ("open", OnConnected);
	}
	
	// Tells us we are connected
	void OnConnected (SocketIOEvent e) {
		Debug.Log ("We are Connected");
		//socket.Emit ("playerhere");
	}

	

	float GetFloatFromJson(JSONObject data, string key){
		return float.Parse(data [key].ToString().Replace("\"", ""));
	}
}
