using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public Text questionText;
    public Transform answerButtonParent;
    public Text scoreText;
    public Text timeText;
    public GameObject questionDisplay;
    public GameObject endGameDisplay;

    private int playerScore = 0;
    private float timeRemaining;

    private DataController dataController;
    private RoundData roundData;
    private QuestionData[] questionPool;
    private bool isRoundActive;
    private int questionIndex;
    private int currentRound = 0;

    public BasicObjectPool answerButtonPool;
    private List<GameObject> answerButtonObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
        dataController = FindObjectOfType<DataController>();

        BeginRound();
	}
	
    private void BeginRound()
    {
        isRoundActive = true;
        questionIndex = 0;
        roundData = dataController.GetCurrentRoundData(currentRound);
        questionPool = roundData.questions;
        timeRemaining = roundData.timeLimitInSeconds;
        ShowQuestions();
        UpdateTime();
    }

	// Update is called once per frame
	private void ShowQuestions () {
        RemoveAnswerButtons();
        QuestionData questionData = questionPool[questionIndex];
        questionText.text = questionData.questionText;

        for(int i = 0; i < questionData.answers.Length; i++)
        {
            GameObject answerButtonObject = answerButtonPool.GetObject();
            answerButtonObject.transform.SetParent(answerButtonParent);
            answerButtonObjects.Add(answerButtonObject);

            AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
            answerButton.SetUp(questionData.answers[i]);
        }
	}

    private void RemoveAnswerButtons()
    {
        while(answerButtonObjects.Count > 0)
        {
            answerButtonPool.ReturnObject(answerButtonObjects[0]);
            answerButtonObjects.RemoveAt(0);
        }
    }

    public void AnswerClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            //Adds to player score
            playerScore += roundData.points;
            scoreText.text = "Score: " + playerScore.ToString();
        }
        if(questionPool.Length > (questionIndex + 1))
        {
            questionIndex++;
            ShowQuestions();
        }
        else
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        isRoundActive = false;
        if (currentRound >= (dataController.allRoundData.Length - 1))
        {
            questionDisplay.SetActive(false);
            endGameDisplay.SetActive(true);
        }
        else
        {
            currentRound++;
            BeginRound();
        }
    }

    public void StartOver()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void UpdateTime()
    {
        timeText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }

    void Update()
    {
        if (isRoundActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTime();
            if(timeRemaining <= 0)
            {
                EndRound();
            }
        }
    }
}
