using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelGrid : MonoBehaviourPunCallbacks
{
    //-------------------------선언부-------------------------
    private Vector2Int foodGridPosition;
    // private Vector2 foodGridPosition;

    public GameObject foodGameObject;
    public GameObject foodGameObjectMini;

    public GameObject pos1;
    public GameObject pos3;
    public GameObject speedUp;
    public GameObject sizeUp;
    public GameObject shield;

    private int width;
    private int height;
    public Snake snake;

    public PhotonView PV;
    public SpriteRenderer SR;

    public int maxFood = 5;  //먹이 대 스폰 개수
    public int maxMiniFood = 10;  //먹이 소 스폰 개수
    public int maxItems = 5;  //아이템, 독 스폰 개수
    public int maxSizeUp = 3; //사이즈업 아이템은 2개

    private int num = 0;
    //   private int[] pos1 = new int[5];
    //   private int[] pos2 = new int[5];
    //    private GameObject[] food = new GameObject[5];
    //-------------------------선언부-------------------------

    //-------------------------생성자-------------------------
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        //방장이면 먹이 5개 생성.
        /*이게 생성자 안에 있으면 NetworkManager에서 Snake를 생성할 때마다
         *즉 플레이어가 방에 들어올 때마다 생성자가 호출되고, 사과 5개씩 생성됨
         * 다른 부분으로 빼거나 NetworkManager의 SpawnPlayer와 함께 둬야할 듯*/
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < maxFood; i++)
                SpawnFood(-30, 30, -20, 20);
            for (int i = 0; i < maxMiniFood; i++)
                SpawnMiniFood(-30, 30, -20, 20);
            for (int i = 0; i < maxItems; i++)
            {
                SpawnPos1();
                SpawnPos3();
                SpawnSpeedUp();
            }
            for(int i = 0; i<maxSizeUp; i++)
            {
                SpawnSizeUp();
               // SpawnShield();
            }

        }
    }
    //-------------------------생성자-------------------------

    //-------------------------get/set-------------------------
    public void Setup(Snake snake)
    {
        this.snake = snake;
    }
    //-------------------------get/set-------------------------

    //-------------------------이하 함수들-------------------------
    public void SpawnPos1()
    {
        int x = Random.Range(-30, 30);
        int y = Random.Range(-20, 20);
        pos1 = PhotonNetwork.Instantiate("pos1", new Vector3(x, y, 0), Quaternion.identity);

    }
    public void SpawnPos3()
    {
        int x = Random.Range(-30, 30);
        int y = Random.Range(-20, 20);
        pos3 = PhotonNetwork.Instantiate("pos3", new Vector3(x, y, 0), Quaternion.identity);

    }

    public void SpawnFood(int xrange1, int xrange2, int yrange1, int yrange2)
    {
        // SpawnFood(-30, 30, -20, 20);
        //화면 x 범위는 -40~38, y범위는 -23~24
        //화면 밖에서 먹이가 스폰되는 일을 막기 위해 랜덤range를 잘 살필 것.
        if (xrange1 < -38) xrange1 = -40;
        else if (xrange2 > 38) xrange2 = 38;
        else if (yrange1 < -23) yrange1 = -23;
        else if (yrange2 > 24) yrange2 = 24;

        int x = Random.Range(xrange1, xrange2);
        int y = Random.Range(yrange1, yrange2);
       
         //생성된 음식도 플레이어끼리 공유하기 위함     
        foodGameObject = PhotonNetwork.Instantiate("FoodApple", new Vector3(x, y, 0), Quaternion.identity);
        //foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;


    }

    public void SpawnMiniFood(int xrange1, int xrange2, int yrange1, int yrange2)
    {
        // SpawnFood(-30, 30, -20, 20);
        //화면 x 범위는 -40~38, y범위는 -23~24
        //화면 밖에서 먹이가 스폰되는 일을 막기 위해 랜덤range를 잘 살필 것.
        if (xrange1 < -38) xrange1 = -40;
        else if (xrange2 > 38) xrange2 = 38;
        else if (yrange1 < -23) yrange1 = -23;
        else if (yrange2 > 24) yrange2 = 24;

        int x = Random.Range(xrange1, xrange2);
        int y = Random.Range(yrange1, yrange2);

        //생성된 음식도 플레이어끼리 공유하기 위함     
        foodGameObjectMini = PhotonNetwork.Instantiate("FoodAppleMini", new Vector3(x, y, 0), Quaternion.identity);
        //foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;


    }

    public void SpawnShield()
    {
        int x = Random.Range(-30, 30);
        int y = Random.Range(-20, 20);
        shield = PhotonNetwork.Instantiate("shield", new Vector3(x, y, 0), Quaternion.identity);

    }


    public void SpawnSpeedUp()
    {
        int x = Random.Range(-30, 30);
        int y = Random.Range(-20, 20);
        speedUp = PhotonNetwork.Instantiate("speedUp", new Vector3(x, y, 0), Quaternion.identity);

    }
    public void SpawnSizeUp()
    {
        int x = Random.Range(-30, 30);
        int y = Random.Range(-20, 20);
        speedUp = PhotonNetwork.Instantiate("sizeUp", new Vector3(x, y, 0), Quaternion.identity);

    }
    /*
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    //   public bool TrySnakeEatFood(Vector2 snakeGridPosition)
    {
        
        int trueCheck = 0;
        //먹이 배열에 있는 gameObject와 snake의 위치가 같으면, 해당 위치의 먹이 파괴.
        for (int i = 0; i < num; i++)
        {
            
            Vector2Int a = new Vector2Int(pos1[i], pos2[i]);
            // Vector2 a = new Vector2(pos1[i], pos2[i]);
            if ((snakeGridPosition.x == a.x) && (snakeGridPosition.y == a.y))
            //if (((snakeGridPosition.x>pos1[i]-0.2)||(snakeGridPosition.x<pos1[i]+0.2))&&((snakeGridPosition.x > pos1[i] - 0.2 )|| (snakeGridPosition.x < pos1[i] + 0.2)))
            {
                // Debug.Log(i);
                trueCheck = 1;
               // Object.Destroy(food[i]);
                //PhotonNetwork.Destroy(food[i]);
                break;
            }
            else
                trueCheck = 0;
        }
        if (trueCheck == 0)
        {
            //Debug.Log(trueCheck);
            return false;
        }
        else
        {
            //Debug.Log(trueCheck);
            return true;
        }

    }
    */

}
