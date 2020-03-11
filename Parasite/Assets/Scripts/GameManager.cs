using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    //네트워크 매니저 인스턴스화
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }
    
    //-------------------------선언부-------------------------
    private static GameManager instance;

    public LevelGrid levelGrid;
    public bool isGoalOpen = false;
    public PhotonView PV;

    private bool avoidStartDead;

    private GameObject[] speeduparray;
    private GameObject[] sizeuparray;
    private GameObject[] pos1array;
    private GameObject[] pos3array;
    private GameObject[] foodarray;

    private List<Vector3> speedupPositionArray;
    private List<Vector3> sizeupPositionArray;
    private List<Vector3> pos1PositionArray;
    private List<Vector3> pos3PositionArray;
    private List<Vector3> foodPositionArray;

    public int test;
    public int[] playerSizeList = { 0, 0, 0, 0 };  //플레이어들의 몸길이 리스트

    public bool[] playerDeadList = { false, false, false, false };  //플레이어의 생사 여부. 없거나 죽으면 false;
    //-------------------------선언부-------------------------

    /*//-------------------------초기 세팅-------------------------
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    //-------------------------초기 세팅-------------------------*/

    private void Start()
    {
        speedupPositionArray = new List<Vector3>();
        sizeupPositionArray = new List<Vector3>();
        pos1PositionArray = new List<Vector3>();
        pos3PositionArray = new List<Vector3>();
        foodPositionArray = new List<Vector3>();
        avoidStartDead=false;

        setPlayerPos();
        SetItemList();
        SetPoisonList();
        SetFoodList();
        Invoke("waitForSecond", 2f);  //2초 대기후 avoidStartDead를 true로 만든다.

        test = 1;  
    }
    //플레이어 스폰
    public void SpawnPlayer(int x, int y)
    {
        PhotonNetwork.Instantiate("Player", new Vector3(x, y, 0), Quaternion.identity);
        PhotonNetwork.LocalPlayer.NickName = LoginManager.playerName;
        //   Debug.Log(PhotonView.ViewID);
        //PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-30, 30), 4, 0), Quaternion.identity);
    }
    public void setPlayerPos()
    {
        if (PhotonNetwork.PlayerList.Length % 4 == 1)
        {
            SpawnPlayer(-38, 22);
        }
        else if (PhotonNetwork.PlayerList.Length % 4 == 2)
        {
            SpawnPlayer(-38, -21);
        }
        else if (PhotonNetwork.PlayerList.Length % 4 == 3)
        {
            SpawnPlayer(36, 22);
        }
        else if (PhotonNetwork.PlayerList.Length % 4 == 0)
        {
            SpawnPlayer(36, -21);
        }

    }


    //방을 떠나면 자동으로 호출되는 메소드
    public override void OnLeftRoom()
    {
        Debug.Log("방나감");
        //UpdateDeadInfo(PV.Owner.ActorNumber);
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        playerDeadList[PhotonNetwork.LocalPlayer.ActorNumber - 1] = false;
        SceneManager.LoadScene("Lobby");
    }

    void Update() 
    {

        //연결이 끊기거나 나갔는지 체크
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            Debug.Log("끊김");
            PhotonNetwork.Disconnect();
        }

        //목표지점 생성 조건 체크
        int max = FindMax(playerSizeList);
        if (max > 19 && isGoalOpen == false) 
        {
            PhotonNetwork.Instantiate("Goal", Vector3.zero, Quaternion.identity);
            PV.RPC("GoalOpen", RpcTarget.AllBuffered);
            //isGoalOpen = true;
        }

      
        if (checkAllDead()&& avoidStartDead)        
        {
            TheEnd();  
        }
        UpdateLists(); //리스트, 배열들 업데이트
        //Debug.Log(playerDeadList[0]+" "+ playerDeadList[1] + " "+ playerDeadList[2] + " "+ playerDeadList[3]);
    }
    //시작하자마자 죽음 방지위한 함수.
    private void waitForSecond()
    {
        avoidStartDead = true;
    }
    //방장 나가면 호출되는 함수
    public override void OnMasterClientSwitched(Player player)
    {
        Debug.Log("OnMasterClientSwitched()");

        //새로운 마스터 클라입장에서 재소환
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("is MasterClient " + PhotonNetwork.IsMasterClient);

            foreach (Vector3 v in speedupPositionArray)
            {
                PhotonNetwork.Instantiate("speedUp", v, Quaternion.identity);
            }
            
            foreach (Vector3 v in sizeupPositionArray)
            {
                PhotonNetwork.Instantiate("sizeUP", v, Quaternion.identity);
            }

            foreach (Vector3 v in pos1PositionArray)
            {
                PhotonNetwork.Instantiate("pos1", v, Quaternion.identity);
            }

            foreach (Vector3 v in pos3PositionArray)
            {
                PhotonNetwork.Instantiate("pos3", v, Quaternion.identity);
            }

            foreach (Vector3 v in foodPositionArray)
            {
                PhotonNetwork.Instantiate("FoodApple", v, Quaternion.identity);
            }
        }
    }

    [PunRPC]
    public void GoalOpen()
    {
        isGoalOpen = true;
    }
    /*
    //연결 끊겼거나 죽었을 때
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
        //SceneChange.ChangePlayScene();
    }*/
    
    //플레이어 몸 길이 정보 갱신
    public void UpdateSizeArray(int PlayerNumber, int size)
    {
        playerSizeList[PlayerNumber - 1] = size;
    }
    
    public void UpdateDeadInfo(int PlayerNumber)
    {
        PV.RPC("UpdateDeadInfo2", RpcTarget.AllBuffered,PlayerNumber);
    }
    //플레이어 죽음 정보 갱신.
    [PunRPC]
    public void UpdateDeadInfo2(int PlayerNumber)
    {
        playerDeadList[PlayerNumber - 1] = false;
    }
    //플레이어 접속 정보 갱신
    public void UpdateConnectInfo(int PlayerNumber)
    {
        PV.RPC("UpdateConnectInfo2", RpcTarget.AllBuffered, PlayerNumber);
    }
    [PunRPC]
    public void UpdateConnectInfo2(int PlayerNumber)
    {
        playerDeadList[PlayerNumber - 1] = true;
    }

     
     
    //플레이어 죽음/접속 현황 반환
    [PunRPC]
    public bool checkAllDead()
    {
        int count = 0;
        for(int i = 0; i < 4; i++)
        {
            //Debug.Log(i+"번째:" + playerDeadList[i]);
            if (!playerDeadList[i]) count++;     //죽었으면 count=+
        }
        if (count == 4) return true;
        else return false;
    }
    //배열 최대값 구하기(몸길이 최대값 찾기)
    public int FindMax(int[] ar)
    {
        int max = 0;
        foreach (int num in ar)
        {
            if (max < num) max = num;
        }
        return max;
    }
    
    //목표지점에 한명이 들어가서 종료
    public void TheEnd()
    {
        PV.RPC("ExitRoom", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ExitRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //모든 플레이어가 동시에 씬 전환하도록
        PhotonNetwork.LeaveRoom();  //방 나가기
        PhotonNetwork.LoadLevel("End");
        //PhotonNetwork.Disconnect();  //연결 끊기
    }

    //main씬에서 버튼 누르면 Lobby씬으로 이동
    public void Goback()
    {
        SceneManager.LoadScene("Lobby");
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
    }

    //-------------------------리스트 세팅-------------------------
    public void SetItemList()
    {
        speeduparray = GameObject.FindGameObjectsWithTag("speedUp");
        foreach (GameObject obj in speeduparray)
        {
            speedupPositionArray.Add(obj.transform.position);
        }
        sizeuparray = GameObject.FindGameObjectsWithTag("sizeUp");
        foreach (GameObject obj in sizeuparray)
        {
            sizeupPositionArray.Add(obj.transform.position);
        }
    }

    public void SetPoisonList()
    {
        pos1array = GameObject.FindGameObjectsWithTag("pos1");
        foreach (GameObject obj in pos1array)
        {
            pos1PositionArray.Add(obj.transform.position);
        }
        pos3array = GameObject.FindGameObjectsWithTag("pos3");
        foreach (GameObject obj in pos3array)
        {
            pos3PositionArray.Add(obj.transform.position);
        }
    }

    public void SetFoodList()
    {
        foodarray = GameObject.FindGameObjectsWithTag("FoodApple");
        foreach (GameObject obj in foodarray)
        {
            foodPositionArray.Add(obj.transform.position);
        }
    }

    public void UpdateLists()
    {
        System.Array.Clear(speeduparray, 0, speeduparray.Length);
        System.Array.Clear(sizeuparray, 0, sizeuparray.Length);
        speedupPositionArray.Clear();
        sizeupPositionArray.Clear();
        SetItemList();
        System.Array.Clear(pos1array, 0, pos1array.Length);
        System.Array.Clear(pos3array, 0, pos3array.Length);
        pos1PositionArray.Clear();
        pos3PositionArray.Clear();
        SetPoisonList();
        System.Array.Clear(foodarray, 0, foodarray.Length);
        foodPositionArray.Clear();
        SetFoodList();
    }
    //-------------------------리스트 세팅-------------------------

}
