using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] Button resetButton;
    [SerializeField] Button replayButton;
    [SerializeField] TMP_Text gameShotsText;
    [SerializeField] TMP_Text gameScoreText;
    [SerializeField] TMP_Text gameTimeText;
    [SerializeField] TMP_Text victoryShotsText;
    [SerializeField] TMP_Text victoryScoreText;
    [SerializeField] TMP_Text victoryTimeText;
    [SerializeField] TMP_Text menuShotsText;
    [SerializeField] TMP_Text menuScoreText;
    [SerializeField] TMP_Text menuTimeText;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject victoryCanvas;
    int shotsTaken;
    public int points;
    float timePassed;
    static InterfaceManager interfaceManagerInstance;
    DateTime startOfGame;
    BallController ballController;
    string saveFilePath;

    class SaveFile
    {
        public int points = 0;
        public int shots = 0;
        public string timespan;

        public SaveFile(int points, int shots, string timespan)
        {
            this.points = points;
            this.shots = shots;
            this.timespan = timespan;
        }
    }

    SaveFile previousGame;

    private void Awake()
    {
        if (interfaceManagerInstance == null)
        {
            interfaceManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
        saveFilePath = Application.persistentDataPath + "/saveData.json";
    }

    public void Start()
    {
        startOfGame = DateTime.Now;
        ballController = FindObjectOfType<BallController>();

        ReadSaveFile();
        
        if (previousGame == null)
        {
            previousGame = new SaveFile(0, 0, TimeSpan.Zero.ToString());
            string jsonFile = JsonUtility.ToJson(previousGame);
            File.WriteAllText(saveFilePath, jsonFile);
        }

        SetUpMenuValues();
    }

    private void Update()
    {
        TimeSpan timeSpent = DateTime.Now - startOfGame;
        gameTimeText.text = timeSpent.ToString(@"hh\:mm\:ss");
    }

    public void ShotBall()
    {
        resetButton.interactable = false;
        replayButton.interactable = false;
        shotsTaken++;
        gameShotsText.text = "Shots: " + shotsTaken;
    }

    public void BallStopped()
    {
        resetButton.interactable = true;
        replayButton.interactable = true;
    }

    public void Scored()
    {
        points++;
        gameScoreText.text = "Score: " + points;
    }

    public void ResetButton()
    {
        ballController.ResetField();
        gameScoreText.text = "Score: 0";
        gameShotsText.text = "Shots: 0";
        startOfGame = DateTime.Now;
        points = 0;
        shotsTaken = 0;
    }

    public void Victory()
    {
        gameCanvas.SetActive(false);
        victoryCanvas.SetActive(true);
        victoryScoreText.text = "Score: " + points;
        victoryShotsText.text = "Shots: " + shotsTaken;
        victoryTimeText.text = gameTimeText.text;
        TimeSpan timeSpent = DateTime.Now - startOfGame;
        gameTimeText.text = timeSpent.ToString(@"hh\:mm\:ss");
        var fileToSave = new SaveFile(points, shotsTaken, timeSpent.ToString(@"hh\:mm\:ss"));

        string jsonFile = JsonUtility.ToJson(fileToSave);

        File.WriteAllText(saveFilePath, jsonFile);

        ReadSaveFile();
        SetUpMenuValues();
    }

    void ReadSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            previousGame = JsonUtility.FromJson<SaveFile>(File.ReadAllText(saveFilePath));
        }
    }

    void SetUpMenuValues()
    {
        menuScoreText.text = "Score: " + previousGame.points.ToString();
        menuShotsText.text = "Shots: " + previousGame.shots.ToString();
        menuTimeText.text = "Time: " + previousGame.timespan.ToString();
    }
}
