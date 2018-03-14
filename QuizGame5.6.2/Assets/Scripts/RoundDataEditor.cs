using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using SocketIO;

public class RoundDataEditor : EditorWindow
{
    string gameDataFilePath = "/StreamingAssets/data.json";
    public AllRoundData editorAllRoundData;
    public RoundData[] editorData;
    private static GameObject server;
    private static SocketIOComponent socket;    

    [MenuItem("Window/Game Data Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(RoundDataEditor)).Show();
    }

    void OnGUI()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty("editorData");
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();

        if (editorData != null)
        {
            if (GUILayout.Button("Save Game Data"))
            {
                SaveGameData();
            }
        }
        if (GUILayout.Button("Save Controller Data to File"))
        {
            SaveDataControllerDataToFile();
        }
        if (GUILayout.Button("Load Game Data From File"))
        {
            LoadGameDataFromFile();
        }
        if (GUILayout.Button("Update Game Data File From Server"))
        {
            LoadGameDataFromServer();
        }
        if (GUILayout.Button("Send Game Data To Server"))
        {
            SendGameData();
        }
    }

    void LoadGameDataFromFile()
    {
        string filePath = Application.dataPath + gameDataFilePath;

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            editorAllRoundData = JsonUtility.FromJson<AllRoundData>(data);
            editorData = editorAllRoundData.roundDataArray;
        }
    }

    public struct StringWrapper { public string wrappedString; }
    void LoadGameDataFromServer()
    {
        string filePath = Application.dataPath + gameDataFilePath;
        StringWrapper wrappedPath = new StringWrapper();
        wrappedPath.wrappedString = filePath;
        string jsonObj = JsonUtility.ToJson(wrappedPath);
        server = GameObject.Find("Server");
        if (server)
        {
            socket = server.GetComponent<SocketIOComponent>();
            socket.Emit("retrieve data", new JSONObject(jsonObj));
        }
    }

    void SaveGameData()
    {
        editorAllRoundData.roundDataArray = editorData;
        string jsonObj = JsonUtility.ToJson(editorAllRoundData);
        string filePath = Application.dataPath + gameDataFilePath;
        File.WriteAllText(filePath, jsonObj);

        GameObject dataControllerObject = GameObject.Find("DataController");
        if (dataControllerObject)
        {
            DataController dataController = dataControllerObject.GetComponent<DataController>();
            dataController.allRoundData = editorAllRoundData;
        }
    }

    void SaveDataControllerDataToFile()
    {
        GameObject dataControllerObject = GameObject.Find("DataController");
        if (dataControllerObject)
        {
            DataController dataController = dataControllerObject.GetComponent<DataController>();
            string jsonObj = JsonUtility.ToJson(dataController.allRoundData);
            string filePath = Application.dataPath + gameDataFilePath;
            File.WriteAllText(filePath, jsonObj);
        }
        else
        {
            Debug.Log("There is no DataController object in the active scene!");
        }
    }

    void SendGameData()
    {
        editorAllRoundData.roundDataArray = editorData;
        string jsonObj = JsonUtility.ToJson(editorAllRoundData);

        server = GameObject.Find("Server");
        if (server)
        {
            socket = server.GetComponent<SocketIOComponent>();
            socket.Emit("send data", new JSONObject(jsonObj));
        }
        else
        {
            Debug.Log("There is no server object in the active scene!");
        }
    }
}