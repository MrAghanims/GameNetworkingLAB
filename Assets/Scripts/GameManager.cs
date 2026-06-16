using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject backToMenuButton;
    public static GameManager Instance;

    [Networked]
    public float MatchTime { get; set; }

    [Networked]
    public bool GameEnded { get; set; }

    [Networked]
    public string WinnerName { get; set; }

    public TMP_Text timerText;
    public TMP_Text winnerText;

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        if (backToMenuButton != null)
        {
            backToMenuButton.SetActive(false);
        }
        if (timerText == null)
            timerText = GameObject.Find("TimerText")
                .GetComponent<TMP_Text>();

        if (winnerText == null)
            winnerText = GameObject.Find("WinnerText")
                .GetComponent<TMP_Text>();

        if (Object.HasStateAuthority)
        {
            MatchTime = 120f;
            GameEnded = false;
        }

        winnerText.gameObject.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (GameEnded)
            return;

        MatchTime -= Runner.DeltaTime;

        if (MatchTime <= 0f)
        {
            MatchTime = 0f;
            EndGame();
        }
    }

    private void Update()
    {
        if (timerText != null)
        {
            timerText.text =
                "Time: " + Mathf.CeilToInt(MatchTime);
        }

        if (GameEnded && winnerText != null)
        {
            winnerText.gameObject.SetActive(true);
            winnerText.text = WinnerName + " WINS!";
        }
    }

    void EndGame()
    {
        GameEnded = true;

        PlayerNetwork[] players =
            FindObjectsOfType<PlayerNetwork>();

        PlayerNetwork winner = null;
        int highestScore = -1;

        foreach (PlayerNetwork p in players)
        {
            if (p.Score > highestScore)
            {
                highestScore = p.Score;
                winner = p;
            }
        }

        foreach (PlayerNetwork p in players)
        {
            p.DisablePlayer();
        }

        if (winner != null)
        {
            WinnerName = winner.PlayerName;
        }
        if (backToMenuButton != null)
        {
            backToMenuButton.SetActive(true);
        }
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}