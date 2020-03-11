using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textPopup : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    TextMeshPro text;
    Color alpha;
    public string message; //초기화는 snake 스크립트에서.
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
        moveSpeed = 1f;
        alphaSpeed = 5f;
        text.text = message;
        Invoke("DestroyObject", 2);
    }

    // Update is called once per frame
    void Update()
    {
        //텍스트가 위로 올라가도록
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        //텍스트가 점점 투명해지도록
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }
    //완전히 투명해진 뒤에는 필요없으므로 파괴
    private void DestroyObject()
    {
        DestroyObject(gameObject);
    }
}
