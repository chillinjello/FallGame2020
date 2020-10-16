using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BoardItem
{
    [SerializeField]
    List<Sprite> WallSprites;

    SpriteRenderer renderer;

    private void Awake() {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = WallSprites[Random.Range(0, WallSprites.Count)];
    }

    public void SetWallPosition(int x, int y) {
        if (!BoardManager.CheckValidCoord(x, y)) return;

        transform.position = BoardManager.GetCoords(x, y);
        xPos = x;
        yPos = y;
    }
}
