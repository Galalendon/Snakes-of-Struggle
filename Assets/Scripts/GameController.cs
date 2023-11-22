using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Game Master")]
    public float totalGameTimeInSeconds = 900;
    public float currentTime;
    public AudioSource audioSource;
    public MovementController defeatedPlayer;


    [Header("Starting Resources")]
    public float startingFood;
    public float startingStone;
    public float startingGold;
    public float startingHealth;

    [Header("Building Costs")]
    public float foodCollectorStoneCost;
    public float stoneCollectorStoneCost;
    public float goldCollectorStoneCost;
    public float flagStoneCost;
    public float flagGoldCost;

    [Header("Victory Slogans")]
    public string player1Victory;
    public string player2Victory;
    public string tieVictory;


    [Header("Player 1")]
    public string player1Name;
    public Color player1Colour;
    public MovementController player1;
    public float player1Score;

    [Header("Player 2")]
    public string player2Name;
    public Color player2Colour;
    public MovementController player2;
    public float player2Score;

    [Header("UI")]
    public TMP_Text timerTextP1;
    public TMP_Text timerTextP2;
    public GameObject endGamePanel;
    public TMP_Text playerWinnerText;
    public TMP_Text player1EndGameScoreText;
    public TMP_Text player2EndGameScoreText;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip victoryMusicP1;
    public AudioClip victoryMusicP2;

    private MovementController[] players;
    private string[] joystickNames;
    private int joystickNameIndex = 0;

    private void Start()
    {
        joystickNames = Input.GetJoystickNames();
        joystickNameIndex = joystickNames.Length;
        endGamePanel.SetActive(false);
        Debug.Log("Number of Joysticks found: " + joystickNameIndex);
        foreach (string jostick in joystickNames)
        {
            Debug.Log(jostick);
        }
        players = FindObjectsOfType<MovementController>();
        foreach (MovementController player in players)
        {
            player.food = startingFood;
            player.stone = startingStone;
            player.gold = startingGold;
            player.health = startingHealth;
            //player.buildingController.foodCollectorStoneCost = foodCollectorStoneCost;
            //player.buildingController.stoneCollectorStoneCost = stoneCollectorStoneCost;
            // player.buildingController.goldCollectorStoneCost = goldCollectorStoneCost;
            //player.buildingController.flagStoneCost = flagStoneCost;
            //player.buildingController.flagGoldCost = flagGoldCost;            

            if (player.gameObject.name == player1Name)
            {
                player1 = player;
                player1Colour = player1.playerColour;
                player1Score = player1.playerScore;
                Debug.Log("Player 1 Found " + player1Name);
            }
            if (player.gameObject.name == player2Name)
            {
                player2 = player;
                player2Colour = player2.playerColour;
                player2Score = player2.playerScore;
                Debug.Log("Player 2 Found: " + player2Name);
            }

            Debug.Log(player);
        }
        currentTime = totalGameTimeInSeconds;        
    }

    private void Update()
    {
        player1Score = player1.playerScore;
        player2Score = player2.playerScore;
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            TimerUI();
        }
        else
        {
            TimerEndGame();
        }

    }
    void TimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerTextP1.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerTextP2.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void TimerEndGame()
    {
        endGamePanel.SetActive(true);
        if (player1Score > player2Score)
        {
            playerWinnerText.color = player1.playerColour;
            playerWinnerText.text = player1Victory;
            audioSource.clip = victoryMusicP1;
            audioSource.Play();
        }
        else if (player2Score > player1Score)
        {
            playerWinnerText.color = player2.playerColour;
            playerWinnerText.text = player2Victory;
            audioSource.clip = victoryMusicP2;
            audioSource.Play();
        }
        else if (player2Score == player1Score)
        {
            playerWinnerText.text = tieVictory;
        }

        player1EndGameScoreText.text = player1Score.ToString();
        player2EndGameScoreText.text = player2Score.ToString();

        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
    }
    public void DefeatEndGame()
    {
        endGamePanel.SetActive(true);
        if (defeatedPlayer == player1)
        {
            playerWinnerText.color = player2.playerColour;
            playerWinnerText.text = player2Victory;
            audioSource.clip = victoryMusicP2;
            audioSource.Play();
        }
        if (defeatedPlayer == player2)
        {
            playerWinnerText.color = player1.playerColour;
            playerWinnerText.text = player1Victory;
            audioSource.clip = victoryMusicP1;
            audioSource.Play();
        }
        player1EndGameScoreText.text = player1Score.ToString();
        player2EndGameScoreText.text = player2Score.ToString();

        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
    }
 

}

