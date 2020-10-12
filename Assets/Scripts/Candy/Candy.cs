using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : BoardItem
{
    [SerializeField]
    public CandyManager.CandyTypes type = CandyManager.CandyTypes.TeleportCandy;

    public void SetPosition(int x, int y) {
        transform.position = BoardManager.GetCoords(x, y);
        xPos = x;
        yPos = y;
    }

    public void PickUpCandy() {
        Managers._player.PickUpCandy(type);

        //remove candy
        Managers._candy.PickUpCandy(this);
    }
}
