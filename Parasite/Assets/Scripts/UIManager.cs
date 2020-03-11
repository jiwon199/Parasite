using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
public class UIManager : MonoBehaviourPunCallbacks, IPunObservable
{
    
    private Text BodySizeText;
    private Text ReadyInfo;
    private Text GoalOpen;
    private Text DeadMsg;
    private Text PlayerNumInfo;

    public int maxPlayer;

    //RectTransform pos;
    [SerializeField] public Snake snake;
    public PhotonView PV;
    RectTransform pos;
    //목표지점 활성화 메시지 출력됙 전엔 true, 출력된 후엔 false로 바뀜(중복출력 방지)
    public bool goalcheck = true; 

    // Start is called before the first frame update
    private void Awake()
    {

        BodySizeText = GameObject.Find("BodySizeText").GetComponent<Text>();
        ReadyInfo = GameObject.Find("ReadyInfo").GetComponent<Text>();
        GoalOpen = GameObject.Find("GoalOpen").GetComponent<Text>();
        DeadMsg = GameObject.Find("DeadMsg").GetComponent<Text>();
        PlayerNumInfo = GameObject.Find("PlayerNumInfo").GetComponent<Text>();

        pos = GameObject.Find("BodySizeText").GetComponent<RectTransform>();
        //  BodySizeText.transform.position = new Vector3(0, 20); 
        pos.anchoredPosition = new Vector2(0, -400);
        //Debug.Log("UIManager 실행");
        ReadyInfo.gameObject.SetActive(false);
        GoalOpen.gameObject.SetActive(false);
        DeadMsg.gameObject.SetActive(false);
        
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        PlayerNumInfo.text = maxPlayer + "인 모드";
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {  
            return;
        }
        BodySizeText.text = "길이 : " + snake.getBodySize().ToString();

        ReadyInfoChange(maxPlayer);

        /*//플레이어 2명 안모이면 대기중 출력
        if (PhotonNetwork.PlayerList.Length < 2 || snake.playerNum<2)
        {
            ReadyInfo.gameObject.SetActive(true);
        }
        else  //2명 모이면 대기중 메시지 false로
        {
            ReadyInfo.gameObject.SetActive(false);
        }*/

        //목표지점 활성화 메시지 출력
        if (GameManager.Instance.isGoalOpen == true && goalcheck)
        {
            GoalOpen.gameObject.SetActive(true);
            goalcheck = false;
            StartCoroutine(Example());
        }
      
      
    }

    public void ReadyInfoChange(int max)
    {
        if (PhotonNetwork.PlayerList.Length < max || snake.playerNum < max)
        {
            ReadyInfo.gameObject.SetActive(true);
        }
        else  //2명 모이면 대기중 메시지 false로
        {
            ReadyInfo.gameObject.SetActive(false);
        }
    }

    public void setDieMsg()
    {
      //  if (!snake.getIsLive())
     //   {
            Debug.Log("죽엇다.");
            DeadMsg.gameObject.SetActive(true);
     //   }
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);//WaitForSeconds객체를 생성해서 반환
        GoalOpen.gameObject.SetActive(false);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
