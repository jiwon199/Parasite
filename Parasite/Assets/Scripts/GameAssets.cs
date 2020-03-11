using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets i;

    private Sprite[] YellowHeadSprites;
    private Sprite[] RedHeadSprites;
    private Sprite[] GreenHeadSprites;
    private Sprite[] BlueHeadSprites;

    private void Awake()
    {
        i = this;
        YellowHeadSprites = Resources.LoadAll<Sprite>("Yellow");
        RedHeadSprites = Resources.LoadAll<Sprite>("Red");
        GreenHeadSprites = Resources.LoadAll<Sprite>("Green");
        BlueHeadSprites = Resources.LoadAll<Sprite>("Blue");
    }

    public Sprite getYellow(int num)
    {
        Sprite yellowHead = YellowHeadSprites[num];
        return yellowHead;
    }

    public Sprite getRed(int num)
    {
        Sprite redHead = RedHeadSprites[num];
        return redHead;
    }

    public Sprite getGreen(int num)
    {
        Sprite greenHead = GreenHeadSprites[num];
        return greenHead;
    }

    public Sprite getBlue(int num)
    {
        Sprite blueHead = BlueHeadSprites[num];
        return blueHead;
    }

}
