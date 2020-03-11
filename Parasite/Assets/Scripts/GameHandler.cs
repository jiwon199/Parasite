using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    private LevelGrid levelGrid;
    public GameObject snakeHeadGameObject;
    private void Start()
    {

        //levelGrid = new LevelGrid(20, 20);

        snake.Setuplv(levelGrid);
        //levelGrid.Setup(snake);

        //SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        //snakeSpriteRenderer.sprite = GameAssets.i.snakeHeadSprite;

    }
}
