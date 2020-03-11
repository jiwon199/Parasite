using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class EndManager : MonoBehaviour
{
    public Button ExitBtn;
    public Image EndImage;
    public Image EndInfo;
    public Text EndReason;

    public Sprite WinImage;
    public Sprite WinText;

    private static string Deadreason = "";
    private static bool Win = false;
    

    // Start is called before the first frame update
    void Start()
    {
         Win = LobbyManager.GetWinPro();
         Deadreason = LobbyManager.GetDiePro();
        if (Win)
        {
            Debug.Log("you win");
            EndInfo.sprite = WinText;
            EndImage.sprite = WinImage;
            EndReason.text = "";
        }
        else
        {
            if (Deadreason == "" || Deadreason == "unkown")
            {
                Deadreason = "상대방이 먼저 골인!";
            }
            EndReason.text = Deadreason;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        SceneManager.LoadScene("Login");
        PhotonNetwork.Disconnect();
    }
}
