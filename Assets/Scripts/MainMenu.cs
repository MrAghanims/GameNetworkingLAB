using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField playerNameInput;

    public static string PlayerName;

    public void JoinGame()
    {
        NetworkManager.LocalPlayerName =
    playerNameInput.text;

        SceneManager.LoadScene("GameScene");
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}