using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class DeadCMmove : MonoBehaviour
{
    Vector3 position;
    public float Speed = 2000f;
    public PhotonView PV;

    private static bool downB;
    private static bool upB;
    private static bool leftB;
    private static bool rightB;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        downB = false;
        upB = false;
        leftB = false;
        rightB = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        HandleMove();
    }
    private void HandleMove()
    {
        if (Input.GetKey(KeyCode.LeftArrow)||leftB)
        {
            resetArrow();
            position.x -= 30*(Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)||rightB)
        {
            resetArrow();
            position.x += 30 * (Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow)||upB)
        {
            resetArrow();
            position.y += 30 * (Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)||downB)
        {
            resetArrow();
            position.y -= 30 * (Speed * Time.deltaTime);
        }
        transform.position = position;
    }
    private void resetArrow()
    {
        downB = false;
        upB = false;
        leftB = false;
        rightB = false;
    }

    public void setDown()
    {
        downB = true;

    }
    public void setUp()
    {
        upB = true;
    }
    public void setLeft()
    {
        leftB = true;
    }
    public void setRight()
    {
        rightB = true;
    }

}
