using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; private set; }

    public void Startup() {
        status = ManagerStatus.Initializing;
        Debug.Log("Started Board Manager...");
        

        status = ManagerStatus.Started;
    }

    const int X_BOARD_SIZE = 6;
    const int Y_BOARD_SIZE = 6;
    

    static public Vector2 GetCoords(int x, int y) {
        Vector2 coord = new Vector2();

        switch (y) {
            case 0:
                coord.y = 2f;
                switch (x) {
                    case 0:
                        coord.x = -3.313f;
                        break;
                    case 1:
                        coord.x = -1.94f;
                        break;
                    case 2:
                        coord.x = -0.59f;
                        break;
                    case 3:
                        coord.x = 0.72f;
                        break;
                    case 4:
                        coord.x = 2.09f;
                        break;
                    case 5:
                        coord.x = 3.5f;
                        break;
                }
                coord.x = -3.34375f + x * 1.375f;
                break;
            case 1:
                coord.y = 1f;
                coord.x = -3.46875f + x * 1.40625f;
                break;
            case 2:
                coord.y = 0f;
                coord.x = -3.5625f + x * 1.4375f;
                break;
            case 3:
                coord.y = -1;
                coord.x = -3.75f + x * 1.5f;
                break;
            case 4:
                coord.y = -2f;
                coord.x = -3.875f + x * 1.5625f;
                break;
            case 5:
                coord.y = -3f;
                coord.x = -4f + x * 1.625f;
                break;
        }
        

        return coord;
    }

    static public Vector2 GetCharacterPosition(Sprite sprite, int x, int y) {
        Vector2 coord = GetCoords(x, y);
        

        return coord;
    }

    static public float FixToPixel (float f) {
        return f  - (f % 0.03125f);
    }

    static public bool CheckValidCoord(int x, int y) {
        if (x >= X_BOARD_SIZE 
            || y >= Y_BOARD_SIZE
            || x < 0 
            || y < 0)
            return false;

        return true;
    }

    static public bool CoinFlip () {
        return (Random.Range(0, 2) > 0.5);
    }
}
