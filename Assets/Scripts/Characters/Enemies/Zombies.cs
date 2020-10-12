using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombies : Enemy
{
    protected override void MoveEnemy() {
        RandomMovement();
    }
}
