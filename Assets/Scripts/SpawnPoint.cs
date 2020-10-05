using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    TextMesh spawnText;

    public Vector2 coord = Vector2.zero;
    
    const int DEFAULT_TIMER = 3;
    int timer = DEFAULT_TIMER;
    public int GetTime() { return timer; }
    public void SetTimer(int time) { timer = time; }
    public void AddTimer(int time) { timer += time; }
    public void SubtractTimer(int time) { timer -= time; }

    string[] timeValues = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

    private void Update() {
        if (timer < 0) {
            spawnText.text = timeValues[0];
        }
        else if (timer > 10) {
            spawnText.text = timeValues[10];
        }
        else {
            spawnText.text = timeValues[timer];
        }
    }

    public void SetPosition(int x, int y) {
        transform.position = BoardManager.GetCoords(x, y);
        coord = new Vector2(x, y);
    }
}
