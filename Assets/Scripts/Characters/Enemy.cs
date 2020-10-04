using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    [SerializeField]
    int attackDamage = BASE_ATTACK_DAMAGE;
    const int BASE_ATTACK_DAMAGE = 1;

    bool stunned = false;

    public override void Attack(int amount) {
        base.Attack(amount);
        stunned = true;
    }

    public virtual void EnemyTurn() {
        if (stunned) {
            stunned = false;
            return;
        }

        if (CheckIfPlayerIsInRange()) {
            AttackPlayer();
        }
        else {
            MoveEnemy();
        }
    }

    protected virtual void AttackPlayer() {
        Managers._turn.Player.Attack(attackDamage);
    }

    protected virtual bool CheckIfPlayerIsInRange() {
        Vector2 playerCoord = Managers._turn.Player.GetPos();
        Vector2 enemyCoord = GetPos();

        if (playerCoord.y == enemyCoord.y) {
            if (playerCoord.x + 1 == enemyCoord.x) {
                return true;
            }
            else if (playerCoord.x - 1 == enemyCoord.x) {
                return true;
            }
        }
        else if (playerCoord.x == enemyCoord.x) {
            if (playerCoord.y + 1 == enemyCoord.y) {
                return true;
            }
            else if (playerCoord.y - 1 == enemyCoord.y) {
                return true;
            }
        }

        return false;
    }

    protected virtual void MoveEnemy() {
        Vector2 distance = Managers._turn.Player.GetPos() - GetPos();

        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y)) {
            if (distance.x > 0) {
                MoveCharacter(MoveDirections.right);
            }
            else {
                MoveCharacter(MoveDirections.left);
            }
        }
        else {
            if (distance.y > 0) {
                MoveCharacter(MoveDirections.down);
            }
            else {
                MoveCharacter(MoveDirections.up);
            }
        }
    }
}
