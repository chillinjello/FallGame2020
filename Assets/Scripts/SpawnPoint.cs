using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : BoardItem
{
    [SerializeField]
    TextMesh spawnText;

    SpriteRenderer sprite;
    
    
    const int DEFAULT_TIMER = 5;
    int timer = DEFAULT_TIMER;
    public int GetTime() { return timer; }
    public void SetTimer(int time) { timer = time; }
    public void AddTimer(int time) { timer += time; }
    public void SubtractTimer(int time) { timer -= time; }

    string[] timeValues = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

    private void Awake() {
        var textRenderer = spawnText.gameObject.GetComponent<MeshRenderer>();
        sprite = GetComponent<SpriteRenderer>();
        textRenderer.sortingOrder = sprite.sortingOrder;
        textRenderer.sortingLayerID = sprite.sortingLayerID;
    }

    private void Update() {
        if (timer <= 0) {
            sprite.color = new Color32(154, 45, 45, 255);
        }
    }

    public void SetPosition(int x, int y) {
        transform.position = BoardManager.GetCoords(x, y);
        xPos = x;
        yPos = y;
    }
}
