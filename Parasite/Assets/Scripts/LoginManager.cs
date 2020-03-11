using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField NickNameInput;
    public Button StartButton;
    public static string playerName;

    public void SignIn()
    {
        SceneManager.LoadScene("Lobby");
        playerName = NickNameInput.text;
    }

    public void GoHowTo()
    {
        SceneManager.LoadScene("Howto");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
