using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

public class Snake : MonoBehaviourPunCallbacks, IPunObservable
{
    //-------------------------선언부-------------------------

    public GameObject RIP;
    public PhotonView PV;
    public Text NickNameText;
    public GameObject startPoint;
    public static int endResult = 0;  //게임 종료 후 결과 정보

    public Sprite faceNumber;  //얼굴 스프라이트
    

    //뱀 이동 방향
    private Vector2Int gridMoveDirection;
    //private Vector2 gridMoveDirection;
 
    private Text DeadMsg;

    public Transform hudPos; // 텍스트팝업 뜨는 위치.
 
    //뱀 위치
    private Vector2Int gridPosition;
    //private Vector2 gridPosition;

    private float gridMoveTimer;
    private float gridMoveTimerMax;  //기생충 속도
    public int snakeBodySize;

    Rigidbody2D rigid;
    CapsuleCollider2D col;
    //뱀 파츠별 위치 정보 배열
    private List<Vector2Int> snakeMovePositionList;
    //private List<Vector2> snakeMovePositionList;
    private bool eatFood;
    private bool SizeUp;
    private bool isLive;
    private bool eatenShield;

    private List<SnakeBodyPart> snakeBodyPartList;
    private List<bool> PlayerList;   //죽으면 false, 없으면 원소 존재하지 않음.

    //플레이어 4명 선언.
    private bool P1;
    private bool P2;
    private bool P3;
    private bool P4;

    //속도증가 아이템 관련 변수
    float timeCheck;
    private bool eatSpeedItem;

    //쉴드 아이템 관련 변수
    float timeCheckShield;
    private bool eatShieldItem;


    public int playerNum;
    public UIManager ui;
    private LevelGrid levelGrid;

    //비석은 플레이어 한 명당 하나 이상 소환될 필요 없으므로, IsRipSpawn이 true면 SpawnRip은 더이상 실행되지 않도록 한다.
    private bool IsRipSpawn;   

    public int rand;  //얼굴 결정 랜덤 변수
    //-------------------------선언부-------------------------

    //-------------------------get/set-------------------------
    public void Setuplv(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    public LevelGrid Getlv()
    {
        return this.levelGrid;
    }
    public int getBodySize()
    {
        return this.snakeBodySize;
    }
   public bool getIsLive()
    {
        return this.isLive;
    }
    //-------------------------get/set-------------------------

    //-------------------------초기 세팅-------------------------
    private void Awake()
    {

        PlayerList = new List<bool>();
        P1 = false;
        P2 = false;
        P3 = false;
        P4 = false;

        //  gridPosition = new Vector2Int(0, 0);   //이거를 바꿔야.

        playerNum = 0;
        //gridPosition = new Vector2(0, 0);

        gridMoveTimerMax = .1f;
        gridMoveTimer = gridMoveTimerMax;

       // gridMoveDirection = new Vector2Int(1, 0);

        //gridMoveDirection = new Vector2(1, 0);

        snakeBodySize = 0;
        snakeMovePositionList = new List<Vector2Int>();
        //snakeMovePositionList = new List<Vector2>();
        
        //뱀 바디 자체의 정보 배열(스프라이트 등)
        snakeBodyPartList = new List<SnakeBodyPart>();

        IsRipSpawn = false;

       //먹이 생성 맵 크기(test)
       levelGrid = new LevelGrid(20, 20); 
       //levelGrid.Setup(this);

        
        //속도증가 아이템 관련 변수
        eatSpeedItem = false;
        timeCheck = 5f;


        //쉴드 아이템 관련 변수
        eatShieldItem = false;
        timeCheckShield = 5f;
        eatenShield = false;


        followPlayer();

        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        eatFood = false;
        isLive= true;
        SizeUp = false;

        //얼굴 모양 결정 랜덤
        rand = Random.Range(0, 10);
        
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        //닉네임 표시, 얼굴 모양, 색 결정
        //  Debug.Log(PV.Owner.ActorNumber); 
        PV.RPC("setPlayerDet", RpcTarget.AllBuffered, this.PV.ViewID);
        if (PV.IsMine)
        {
            NickNameText.text = PhotonNetwork.NickName;
            NickNameText.color = Color.green;
            PV.RPC("SetHead", RpcTarget.AllBuffered, this.PV.ViewID, rand);          
        }
        else
        {
            NickNameText.text = PV.Owner.NickName;
            NickNameText.color = Color.red;
        }
          
        /*
         if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                Debug.Log(PV.Owner.ActorNumber + " : " + i + " : " + PlayerList[i]);
            }
        }
        */


    }
    //-------------------------초기 세팅-------------------------
    [PunRPC]
    private void setPlayerDet(int viewid)
    {

        //화면 x 범위는 -40~38, y범위는 -23~24
        if ((int)(viewid / 1000) % 4 == 1)
        {
            gridPosition = new Vector2Int(-38, 22);
            //  GameManager.Instance.SpawnPlayer(-38,22);
            gridMoveDirection = new Vector2Int(1, 0);
            PV.RPC("avoidDelay", RpcTarget.AllBuffered, 1);
            GameManager.Instance.UpdateConnectInfo(PV.Owner.ActorNumber);
        }

        else if ((int)(viewid / 1000) % 4 == 2)
        {
            gridPosition = new Vector2Int(-38, -21);
            //  GameManager.Instance.SpawnPlayer(-38, -21);
            gridMoveDirection = new Vector2Int(1, 0);
            PV.RPC("avoidDelay", RpcTarget.AllBuffered, 2);
            GameManager.Instance.UpdateConnectInfo(PV.Owner.ActorNumber);

        }
        else if ((int)(viewid / 1000) % 4 == 3)
        {
          
            gridPosition = new Vector2Int(36, 22);
            // GameManager.Instance.SpawnPlayer(36, 22);
            gridMoveDirection = new Vector2Int(-1, 0);
            PV.RPC("avoidDelay", RpcTarget.AllBuffered, 3);
            GameManager.Instance.UpdateConnectInfo(PV.Owner.ActorNumber);
        }
        else if ((int)(viewid / 1000) % 4 == 0)
        {
           
            gridPosition = new Vector2Int(36, -21);
            //    GameManager.Instance.SpawnPlayer(36, -21);
            gridMoveDirection = new Vector2Int(-1, 0);
            PV.RPC("avoidDelay", RpcTarget.AllBuffered, 0);
            GameManager.Instance.UpdateConnectInfo(PV.Owner.ActorNumber);
        }


    }
    //p2가 방 참가 성공이 되고 게임화면으로 넘어가기 직전 p1이 게임을 시작해버려서 생기는 딜레이를 막는 함수.
    [PunRPC]
    private void avoidDelay(int i)
    {
        //Debug.Log(playerNum);
        if (i == 1&&P1==false) {
            P1 = true;
            // playerNum++;
            PV.RPC("addPlayerNum", RpcTarget.AllBuffered);          
        }
        else if (i == 2&&P2 == false) {
            P2 = true;
            //    playerNum++;
            PV.RPC("addPlayerNum", RpcTarget.AllBuffered);
           
        }
        else if (i == 3 && P3 == false) {
            P3 = true;
            PV.RPC("addPlayerNum", RpcTarget.AllBuffered);
        }
        else if (i == 0 && P4 == false) {
            P4 = true;
            PV.RPC("addPlayerNum", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void addPlayerNum()
    {
        playerNum++;
    }
    [PunRPC]
    private void SetHead(int viewid, int rand)
    {
        if ((int)(viewid / 1000) % 4 == 1)
        {
            faceNumber = GameAssets.i.getYellow(rand);
        }
        else if ((int)(viewid / 1000) % 4 == 2)
        {
            faceNumber = GameAssets.i.getRed(rand);
        }
        else if ((int)(viewid / 1000) % 4 == 3)
        {
            faceNumber = GameAssets.i.getGreen(rand);
        }
        else if ((int)(viewid / 1000) % 4 == 0)
        {
            faceNumber = GameAssets.i.getBlue(rand);
        }

        PhotonView.Find(viewid).gameObject.GetComponent<SpriteRenderer>().sprite = faceNumber;
    }

    //-------------------------이하 함수들-------------------------
    public void Update()
    {
         
       // Debug.Log(eatenShield);

        //플레이어 수가 바뀌면 변경되는 것들->아래줄+로비매니저 maxPlayer 수, UI매니저 대기 메세지 조건창.
        if (!PV.IsMine || PhotonNetwork.PlayerList.Length < 2||playerNum<2)
        //if (!PV.IsMine)
        {
            return;
        }
        //Debug.Log(playerNum);
        OnTriggerEnter2D(col);
        if (eatSpeedItem) checkItemTime();
        if (eatShieldItem) checkShield();



        //카메라 플레이어 따라 이동
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }

        HandleInput();  //키입력->뱀 이동 방향 결정
        HandleGridMovement();  //방향대로 이동
    }

    //콜라이더 체크
    void OnTriggerEnter2D(Collider2D col)
    {

        //쉴드를 먹음
        if (col.CompareTag("shield"))
        {
            Debug.Log("쉴드");
            PV.RPC("popMessage", RpcTarget.AllBuffered, "보호막!");
            int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;

            PV.RPC("eatShield", RpcTarget.AllBuffered, viewid);
            eatenShield = true;
        }





        //먹이를 먹음 
        if (col.CompareTag("FoodApple"))
        {
            //충돌한 오브젝트의 아이디를 가져오기
            int viewid= col.gameObject.GetComponent<PhotonView>().ViewID;
         
            //eatApple함수를 실행 마스터가 아닌경우 인자를 넘겨주며 처리해주기를 요청(아마)
            PV.RPC("eatApple", RpcTarget.AllBuffered, viewid);
        }

        //왼쪽벽
        if (col.CompareTag("leftWall"))
        {
            GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
            PV.RPC("spawnRip", RpcTarget.AllBuffered, 1);
            ui.setDieMsg();
            DeadHandleInput();
            PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");

            PV.RPC("eatwall", RpcTarget.AllBuffered);
        }
        //위쪽벽
        if (col.CompareTag("upWall"))
        {
            GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
            PV.RPC("spawnRip", RpcTarget.AllBuffered, 2);
            ui.setDieMsg();
            DeadHandleInput();
            PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");

            PV.RPC("eatwall", RpcTarget.AllBuffered);
        }
        //오른쪽벽
        if (col.CompareTag("rightWall"))
        {
            GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
            PV.RPC("spawnRip", RpcTarget.AllBuffered, 3);
            ui.setDieMsg();
            DeadHandleInput();
            PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");

            PV.RPC("eatwall", RpcTarget.AllBuffered);
        }
        //아래쪽벽
        if (col.CompareTag("downWall"))
        {
            GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
            PV.RPC("spawnRip", RpcTarget.AllBuffered, 4);
            ui.setDieMsg();
            DeadHandleInput();
            PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");

            PV.RPC("eatwall", RpcTarget.AllBuffered);
        }

        if (eatenShield  == false)
        {
            //즉사독을 먹음
            if (col.CompareTag("pos1"))
            {
                GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
                PV.RPC("spawnRip", RpcTarget.AllBuffered, 0);
                ui.setDieMsg();
                DeadHandleInput();
                PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");
                int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
                
                PV.RPC("eatPos1", RpcTarget.AllBuffered, viewid);
            }
            //길이 절반 독을 먹음
            if (col.CompareTag("pos3"))
            {
                PV.RPC("popMessage", RpcTarget.AllBuffered, "길이줄어듦!");
                int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;

                PV.RPC("eatPos3", RpcTarget.AllBuffered, viewid);
            }
            //노란색 몸에 닿음.
            if (col.CompareTag("yellow"))
            {
                //자기몸이면 죽지 않는다.
                if (PV.Owner.ActorNumber % 4 == 1)
                {
                    //   Debug.Log("자기몸에 닿음");
                }
                //남의 몸이면 죽는다.
                else
                {
                    GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
                    PV.RPC("spawnRip", RpcTarget.AllBuffered, 0);
                    ui.setDieMsg();
                    DeadHandleInput();
                    PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");
                    int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
                    LobbyManager.SetDiePro("상대방 몸에 부딪혀 사망!");

                    PV.RPC("eatwall", RpcTarget.AllBuffered);
                }
            }
            //빨간색 몸에 닿음.
            if (col.CompareTag("red"))
            {
                //자기몸이면 죽지 않는다.
                if (PV.Owner.ActorNumber % 4 == 2)
                {
                    //  Debug.Log("자기몸에 닿음");
                }
                //남의 몸이면 죽는다.
                else
                {
                    GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
                    PV.RPC("spawnRip", RpcTarget.AllBuffered, 0);
                    ui.setDieMsg();
                    DeadHandleInput();
                    PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");
                    int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
                    LobbyManager.SetDiePro("상대방 몸에 부딪혀 사망!");

                    PV.RPC("eatwall", RpcTarget.AllBuffered);
                }
            }
            //초록색 몸에 닿음.
            if (col.CompareTag("green"))
            {
                //자기몸이면 죽지 않는다.
                if (PV.Owner.ActorNumber % 4 == 3)
                {
                    //  Debug.Log("자기몸에 닿음");
                }
                //남의 몸이면 죽는다.
                else
                {
                    GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
                    PV.RPC("spawnRip", RpcTarget.AllBuffered, 0);
                    ui.setDieMsg();
                    DeadHandleInput();
                    PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");
                    int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
                    LobbyManager.SetDiePro("상대방 몸에 부딪혀 사망!");

                    PV.RPC("eatwall", RpcTarget.AllBuffered);
                }
            }
            //파란색 몸에 닿음.
            if (col.CompareTag("blue"))
            {
                //자기몸이면 죽지 않는다.
                if (PV.Owner.ActorNumber % 4 == 0)
                {
                    //   Debug.Log("자기몸에 닿음");
                }
                //남의 몸이면 죽는다.
                else
                {
                    GameManager.Instance.UpdateDeadInfo(PV.Owner.ActorNumber);
                    PV.RPC("spawnRip", RpcTarget.AllBuffered, 0);
                    ui.setDieMsg();
                    DeadHandleInput();
                    PV.RPC("popMessage", RpcTarget.AllBuffered, "즉사함!");
                    int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
                    LobbyManager.SetDiePro("상대방 몸에 부딪혀 사망!");

                    PV.RPC("eatwall", RpcTarget.AllBuffered);
                }
            }
        }
        //길이증가 아이템을 먹음
        if (col.CompareTag("sizeUp"))
        {
            
            PV.RPC("popMessage", RpcTarget.AllBuffered, "길이두배!");
            int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;
            PV.RPC("eatSizeUp", RpcTarget.AllBuffered, viewid);  //없애고
            int CurrentBodySize = snakeBodySize ;
            for (int i = 0; i < CurrentBodySize; i++)
            {
                PV.RPC("eatSizeUp2", RpcTarget.AllBuffered);  //늘인다.
                
            }


        }
        //속도증가 아이템을 먹음
        if (col.CompareTag("speedUp"))
        {
            PV.RPC("popMessage", RpcTarget.AllBuffered, "빨라짐!");
            int viewid = col.gameObject.GetComponent<PhotonView>().ViewID;

            PV.RPC("eatSpeedUp", RpcTarget.AllBuffered, viewid);

        }
        //목표지점에 골인
        if (GameManager.Instance.isGoalOpen == true && col.CompareTag("Goal"))
        {
            LobbyManager.SetWinPro(true);
            LobbyManager.SetDiePro("이김");
            GameManager.Instance.TheEnd();
        }
    }


    [PunRPC]
    public void spawnRip(int i)//i의 숫자로 왜 죽었는지 판별. (독을 먹었는지..어느 벽에 닿았는지..)  
    {
        if (PV.IsMine&&IsRipSpawn==false)
        {
            IsRipSpawn = true;
            if (i == 0) //즉사독
            {
                if(eatenShield == false)
                {
                    RIP = PhotonNetwork.Instantiate("RIP", new Vector3(hudPos.position.x, hudPos.position.y + 5, 0), Quaternion.identity);
                    LobbyManager.SetDiePro("즉사독으로 사망!");
                }
                
            }
            else if (i == 1) //왼벽
            {
                RIP = PhotonNetwork.Instantiate("RIP", new Vector3(hudPos.position.x + 3, hudPos.position.y + 5, 0), Quaternion.identity);
                LobbyManager.SetDiePro("왼쪽 벽에 부딪힘!");
            }
            else if (i == 2) //위벽
            {
                RIP = PhotonNetwork.Instantiate("RIP", new Vector3(hudPos.position.x, hudPos.position.y + 2, 0), Quaternion.identity);
                LobbyManager.SetDiePro("위쪽 벽에 부딪힘!");
            }
            else if (i == 3) //오른벽
            {
                RIP = PhotonNetwork.Instantiate("RIP", new Vector3(hudPos.position.x - 3, hudPos.position.y + 5, 0), Quaternion.identity);
                LobbyManager.SetDiePro("오른쪽 벽에 부딪힘!");
            }
            else if (i == 4) //아래벽
            {
                RIP = PhotonNetwork.Instantiate("RIP", new Vector3(hudPos.position.x, hudPos.position.y + 8, 0), Quaternion.identity);
                LobbyManager.SetDiePro("아래쪽 벽에 부딪힘!");
            }
        }
             
    }
    [PunRPC]
    private void eatwall()
    {
        PhotonNetwork.Destroy(this.gameObject); //snake머리 없애고

        //몸통들을 없앤다.
        //리스트인데 이렇게 for문으로 지우면 덜 없어져야되는거 아닌가...? 일단 잘돌아가길래 pass..
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            PhotonNetwork.Destroy(snakeBodyPartList[i].snakeBodyGameObject);
        }

        //독 먹으면 죽은거 반영
        GameManager.Instance.UpdateSizeArray(PV.Owner.ActorNumber, 0);

        //죽은 위치 근처에 자기 길이만큼 먹이를 뿌린다. 
        for (int i = 0; i < snakeBodySize; i++)
        {
            levelGrid.SpawnFood(gridPosition.x - 3, gridPosition.x + 3, gridPosition.y - 3, gridPosition.y + 3);
        }
        isLive = false; //죽은 상태로 변경

    }
    [PunRPC]
    public void popMessage(string msg)
    {

        GameObject hudText = PhotonNetwork.Instantiate("popup", new Vector3(hudPos.position.x, hudPos.position.y, 0), Quaternion.identity);
       
        hudText.GetComponent<textPopup>().message = msg;
    }
    [PunRPC]
    private void eatApple(int viewid)
    {
        // PhotonNetwork.Destroy(col.gameObject);

        //파라미터로 받은 viewid로 먹은 먹이를 찾아 없앤다.
        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);

        eatFood = true;
    }
    [PunRPC]
    private void eatSizeUp(int viewid)
    {
        
        //파라미터로 받은 viewid로 먹은 아이템 찾아 없앤다.
        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);

 
    }
    
    [PunRPC]
    private void eatSizeUp2() 
    {
        SizeUp = true;
        if ((SizeUp)&& (PV.IsMine) && snakeBodySize < 99)
        {
            snakeBodySize++;
            CreateSnakeBodyPart(PV.Owner.ActorNumber);
            GameManager.Instance.UpdateSizeArray(PV.Owner.ActorNumber, snakeBodySize);
            snakeMovePositionList.Insert(0, gridPosition);  //흠..
            SizeUp = false;
            
        }
    }


    [PunRPC]
    private void eatShield(int viewid)
    {
        Debug.Log("쉴드" + viewid);
        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);
        eatShieldItem = true;

    }






    [PunRPC]
    private void eatPos1(int viewid)
    {
        PhotonNetwork.Destroy(this.gameObject); //snake머리 없애고
        

        //파라미터로 받은 viewid로 먹은 독을 찾아 없앤다.
        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);

        //몸통들을 없앤다.
        //리스트인데 이렇게 for문으로 지우면 덜 없어져야되는거 아닌가...? 일단 잘돌아가길래 pass..
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            PhotonNetwork.Destroy(snakeBodyPartList[i].snakeBodyGameObject);
        }

        //독 먹으면 죽은거 반영
        GameManager.Instance.UpdateSizeArray(PV.Owner.ActorNumber, 0);

        //죽은 위치 근처에 자기 길이만큼 먹이를 뿌린다. 
        for (int i = 0; i < snakeBodySize; i++)
        {
            levelGrid.SpawnFood(gridPosition.x - 3, gridPosition.x + 3, gridPosition.y - 3, gridPosition.y + 3);
        }

        isLive = false; //죽은 상태로 변경

    }
    [PunRPC]
    private void eatPos3(int viewid)
    {

        //파라미터로 받은 viewid로 먹은 독을 찾아 없앤다.
        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);

        int CurrentBodySize = snakeBodySize - 1;     //snakeBodySize는 1부터 시작하기 때문에 관리 쉽게하려고 -1 함.
        int mid = CurrentBodySize / 2;

        //루프를 돌면서 절반의 몸통을 지우는데, 몸통 길이가 짝수개인지 홀수개인지에 따라 돌아야 할 루프문 수가 다르므로 if로 분기.
        if (CurrentBodySize % 2 == 1)
        {
            for (int i = CurrentBodySize; i > mid; i--)
            {
                snakeBodySize = snakeBodySize - 1;
                PhotonNetwork.Destroy(snakeBodyPartList[i].snakeBodyGameObject);
                snakeBodyPartList.RemoveAt(i);
            }

        }
        else
        {
            for (int i = CurrentBodySize; i > mid - 1; i--)
            {
                snakeBodySize = snakeBodySize - 1;
                PhotonNetwork.Destroy(snakeBodyPartList[i].snakeBodyGameObject);
                snakeBodyPartList.RemoveAt(i);
            }
        }
        //독먹으면 길이 반영
        GameManager.Instance.UpdateSizeArray(PV.Owner.ActorNumber, snakeBodySize);
    }
    [PunRPC]
    private void eatSpeedUp(int viewid)
    {

        PhotonView view = PhotonView.Find(viewid);
        PhotonNetwork.Destroy(view);
        eatSpeedItem = true;

    }
    
    //속도 증가 아이템 효과 지속
    private void checkItemTime()
    {
        //Debug.Log(timeCheck);
        timeCheck = timeCheck - Time.deltaTime;

        if (timeCheck < 0)  //효과끝
        {

            timeCheck = 5f;
            gridMoveTimerMax = .1f;
            eatSpeedItem = false;
        }
        //효과지속중
        else
        {
            gridMoveTimerMax = .05f;
        }

    }
    //쉴드 아이템 효과

    private void checkShield()
    {

        timeCheckShield = timeCheckShield - Time.deltaTime;

        if (timeCheckShield < 0)
        {

            timeCheckShield = 5f;
            eatShieldItem = false;
            eatenShield = false;
            PV.RPC("popMessage", RpcTarget.AllBuffered, "보호막끝남!");

        }

    }



    //방향키 입력
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection.y != -1)
            {  //아래로 가고있는데 바로 위로 갈 수 없음
                gridMoveDirection.x = 0;
                gridMoveDirection.y = +1;
              //  setUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //위로가고있는데 바로 아래로 갈 수 없음.
            if (gridMoveDirection.y != +1)
            {
                gridMoveDirection.x = 0;
                gridMoveDirection.y = -1;
              //  setDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //오른쪽으로 가고 있는데 바로 왼쪽으로 갈 수 없음.
            if (gridMoveDirection.x != +1)
            {
                gridMoveDirection.x = -1;
                gridMoveDirection.y = 0;
             //   setLeft();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //왼쪽으로 가고 있는데 바로 오른쪽으로 갈 수 없음.
            if (gridMoveDirection.x != -1)
            {
                gridMoveDirection.x = +1;
                gridMoveDirection.y = 0;
              //  setRight();
            }
        }

    }
    private void followPlayer()
    {
        //카메라 플레이어 따라 이동
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
    }
    //방향키를 연속으로 입력받지 않아도 초마다 계속 나아가게 함.   
    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            //뱀 위치 정보 갱신
            //머리가 0번, 머리의 위치에 뱀 위치 전달
            snakeMovePositionList.Insert(0, gridPosition);  
            //뱀 머리 위치 = 원래 머리 위치 + 방향
            gridPosition += gridMoveDirection;

           
            //먹이 먹는거 성공했으면몸크기 ++해줌
           // if (snakeAteFood)
           if(eatFood && snakeBodySize < 99)
            {
                snakeBodySize++;
                CreateSnakeBodyPart(PV.Owner.ActorNumber);
                GameManager.Instance.UpdateSizeArray(PV.Owner.ActorNumber, snakeBodySize);
                eatFood = false;
            }
            
            //snakeBody 길어지는거 관리
            //뱀 위치정보 배열의 크기가 뱀 길이보다 클 때 뒷부분을 제거해주는 역할
             
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
           

            //snake의 위치 갱신, 머리방향조절.

            /*
             //-------------------------벽 관련 코드-------------------------
             if ((gridPosition.y > 24) || (gridPosition.y < -23))
             {
                 if ((gridPosition.x > 38) || (gridPosition.x < -40))
                     leftWall();
                 else
                     upWall();

             }
             //왼,오른 벽
             else if ((gridPosition.x > 38) || (gridPosition.x < -40))
             {
                 if ((gridPosition.y > 24) || (gridPosition.y < -23))
                     upWall();
                 else
                     leftWall();

             }
             else
             {
                 transform.position = new Vector3(gridPosition.x, gridPosition.y);
                 UpdateSnakeBodyParts();
                 //벽에 안 부딪혔을때 Wallcount 변수를 증가시켜서, 벽에 부딪혔을때 왼쪽으로 갈지 오른쪽으로 갈지 정한다. 
                 //Update를 쓰다보니 랜덤인수나 다른 방법은 문제가 좀 있어서;;
                 Wallcount++;
             }
             //-------------------------벽 관련 코드-------------------------
             */
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            UpdateSnakeBodyParts();
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) - 90);
        }
    }

    //죽었을때도 카메라 이동은 할 수 있다.
    private void DeadHandleInput()
    {
        GameObject DeadPos;
        //원래는 카메라가 snake를 따라가는데, 죽으면 죽은 위치에 초점 역할을 하는 새 오브젝트를 만들어 그걸 따라가도록 한다. 
        DeadPos=PhotonNetwork.Instantiate("DeadCMpos", new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
        
        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = DeadPos.transform;
            CM.LookAt = DeadPos.transform;
        }
    }
     
    //--------------------------------------------------
    //뱀 꼬리 쪽에 몸통 새로 붙임
    private void CreateSnakeBodyPart(int number)
    {
        SnakeBodyPart a = new SnakeBodyPart(snakeBodyPartList.Count, number);
        snakeBodyPartList.Add(a);
    }
    private void UpdateSnakeBodyParts()
    {
        //몸통들을 적절한 위치에.
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            if (snakeBodyPartList[i].snakeBodyGameObject != null)
            {
                snakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
            }
        }
    }

    //뱀의 머리방향 조절 위한 함수. 백터->앵글
    private float GetAngleFromVector(Vector2Int dir)
    //  private float GetAngleFromVector(Vector2 dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    public Vector2Int GetGridPosition()
    //public Vector2 GetGridPosition()
    {
        return gridPosition;
    }

    private class SnakeBodyPart
    {
        private Vector2Int gridPosition;
        // private Vector2 gridPosition;
        public GameObject snakeBodyGameObject;
        public Sprite bodyNumber;
        public Transform transform;
        
        public SnakeBodyPart(int bodyIndex, int Actornum)
        {
            if(Actornum % 4 == 1)
            {
                snakeBodyGameObject = PhotonNetwork.Instantiate("SnakeBody1", new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
            }
        
            else if(Actornum % 4 == 2)
            {
                snakeBodyGameObject = PhotonNetwork.Instantiate("SnakeBody2", new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
            }
            else if (Actornum % 4 == 3)
            {
                snakeBodyGameObject = PhotonNetwork.Instantiate("SnakeBody3", new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
            }
            else if (Actornum % 4 == 0)
            {
                snakeBodyGameObject = PhotonNetwork.Instantiate("SnakeBody4", new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
            }
            
           // snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;  //sorting order설정->에러????
           //어느정도 길어지면 background에 가려져서 몸이 안보이게 되므로 주석처리.

            transform = snakeBodyGameObject.transform;  //해당 오브젝트의 정보를 transform에
        }

        public void SetGridPosition(Vector2Int gridPosition)
        // public void SetGridPosition(Vector2 gridPosition)
        {
            this.gridPosition = gridPosition;
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
        }
    }

    //동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
    }

}
