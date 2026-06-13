using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject winnerTextObject;
    public TMP_Text winnerText;

    bool gameEnded = false;

    public void EndGame(string winnerName)
    {
        if (gameEnded) return;

        gameEnded = true;

        StartCoroutine(EndRoutine(winnerName));
    }

    IEnumerator EndRoutine(string winnerName)
    {
        winnerTextObject.SetActive(true);

        winnerText.text =
            winnerName + " WINS!";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}