using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frankenstein : Enemy
{
    bool moveTurn = false;

    protected override void Awake() {
        base.Awake();
        attackDamage = 2;
        currentHealth = 4;
    }

    public override bool EnemyAttack() {
        if (moveTurn && base.EnemyAttack()) {
            return true;
        }
        return false;
    }

    public override void EnemyMove() {
        if (moveTurn)
            base.EnemyMove();

        moveTurn = !moveTurn;
    }
}
