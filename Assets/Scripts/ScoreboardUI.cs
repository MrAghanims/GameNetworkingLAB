using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    public TextMeshProUGUI scoreboardText;

    void Update()
    {
        PlayerNetwork[] players =
            FindObjectsOfType<PlayerNetwork>();

        string board = "";

        foreach (PlayerNetwork p in players)
        {
            board +=
                p.PlayerName +
                " | HP: " +
                p.HP +
                " | Score: " +
                p.Score +
                "\n";
        }

        scoreboardText.text = board;
    }
}