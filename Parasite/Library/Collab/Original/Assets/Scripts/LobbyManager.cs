using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    public Text InfoText;
    public Button JoinButton;
    public Button MakeRoomBtn;
    public GameObject turnSnake;

    public GameObject PopUpPanel;
    public Button TwoBtn;
    public Button ThreeBtn;
    public Button FourBtn;
    public GameObject SelectImage;

    public static int playerNumber = 1;

    //public static bool winorlose;
    //public static string diereason;

    private float xSpeed = 360f;
    private bool stopturn = false;

    /*void Awake()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        //PhotonNetwork.autoJoinLobby = false;
        //PhotonNetwork.automaticallySyncScene = true;
    }*/

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToMaster();

        PopUpPanel.SetActive(false);
        JoinButton.interactable = false;
        InfoText.text = "마스터 서버에 접속중...";
        Debug.Log("접속중");
    }

    public override void OnConnectedToMaster()
    {
        JoinButton.interactable = true;
        InfoText.text = "온라인 : 마스터 서버와 연결됨";
        stopturn = true;
        PhotonNetwork.LocalPlayer.NickName = LoginManager.playerName;
        //Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("마스터에 연결");
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon services");
        InfoText.text = "연결 거부";
    }

    public void Update()
    {
        if(stopturn == false)
        {
            turnSnake.transform.Rotate(Vector3.back, Space.Self);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        JoinButton.interactable = false;
        InfoText.text = "오프라인 : 접속 재시도 중...";
        //Debug.Log("접속 재시도");

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        //Debug.Log("connect함수 실행");
        stopturn = false;
        JoinButton.interactable = false;
        if (PhotonNetwork.IsConnected) {
            InfoText.text = "룸에 접속...";
            Debug.Log("연결시도");
            PhotonNetwork.JoinRandomRoom(null,0);
        }
        else
        {
            InfoText.text = "오프라인 : 접속 재시도 중...";
            Debug.Log("연결안됨");
            PhotonNetwork.ConnectUsingSettings();
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "win", false }, { "dieReason", "unkown" } });
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["win"]);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        InfoText.text = "빈 방이 없음, 새로운 방 생성...";
        Debug.Log("방 생성");
        stopturn = true;

        PopUpPanel.SetActive(true);
        //SelectImage.SetActive(false);
        JoinButton.gameObject.SetActive(false);
        //MakeRoomBtn.interactable = false;

        /*if (playerNumber > 1)
        {
            MakeRoomBtn.interactable = true;
        }*/

        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2, CleanupCacheOnLeave = true });
    }

    public void MakeRoom()
    {
        if (playerNumber < 2 || playerNumber > 4)
        {
            return; 
        }
        else
        {
            RoomOptions roomOptions = new RoomOptions();
            //roomOptions.MaxPlayers = 0;
            roomOptions.CleanupCacheOnLeave = true;
            if (playerNumber == 2)
            {
                roomOptions.MaxPlayers = 2;
            }
            else if (playerNumber == 3)
            {
                roomOptions.MaxPlayers = 3;
            }
            else if (playerNumber == 4)
            {
                roomOptions.MaxPlayers = 4;
            }
            PhotonNetwork.CreateRoom("CustomPropertiesRoom", roomOptions, null);
        }
        stopturn = false;
    }

    public override void OnJoinedRoom()
    {
        InfoText.text = "방 참가 성공";
        Debug.Log("방 들어감");
       
        PhotonNetwork.LoadLevel("Main"); //접속한 모든 사람들에게 같은 씬 로드, 자동 동기화
    }

    public void SetPlayerNum2()
    {
        playerNumber = 2;
        //SelectImage.SetActive(true);
        SelectImage.transform.position = new Vector3(-32,3,0);
    }

    public void SetPlayerNum3()
    {
        playerNumber = 3;
        //SelectImage.SetActive(true);
        SelectImage.transform.position = new Vector3(-8, 3, 0);
    }

    public void SetPlayerNum4()
    {
        playerNumber = 4;
        //SelectImage.SetActive(true);
        SelectImage.transform.position = new Vector3(16, 3, 0);
    }

    public static int GetPlayerNumber()
    {
        return playerNumber;
    }


    public static void SetWinPro(bool bo)
    {
        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        playerCP["win"] = bo;
    }

    public static void SetDiePro(string reason)
    {
        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        playerCP["dieReason"] = reason;
    }

    public static bool GetWinPro()
    {
        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        return (bool)playerCP["win"];
    }

    public static string GetDiePro()
    {
        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        return (string)playerCP["dieReason"];
    }

    public void Goback()
    {
        SceneManager.LoadScene("Login");
        PhotonNetwork.Disconnect();
    }
}
