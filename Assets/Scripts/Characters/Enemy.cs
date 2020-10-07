using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    [SerializeField]
    int attackDamage = BASE_ATTACK_DAMAGE;
    const int BASE_ATTACK_DAMAGE = 1;

    bool stunned = false;
    bool attacked = false;

    public override void Attack(int amount) {
        base.Attack(amount);
        stunned = true;
    }

    public virtual void EnemyAttack() {
        if (stunned) {
            return;
        }
        MoveDirections playerDirection = CheckIfPlayerIsInRange();
        if (playerDirection != MoveDirections.none) {
            AttackPlayer(playerDirection);
            attacked = true;
        }
    }

    public virtual void EnemyMove() {
        if (stunned || attacked) {
            stunned = false;
            attacked = false;
            return;
        }

        MoveEnemy();
    }

    protected virtual void AttackPlayer(MoveDirections direction) {
        Managers._turn.Player.Attack(attackDamage);
        AttackAnimation(direction);
    }

    protected virtual MoveDirections CheckIfPlayerIsInRange() {
        Vector2 playerCoord = Managers._turn.Player.GetPos();
        Vector2 enemyCoord = GetPos();

        if (playerCoord.y == enemyCoord.y) {
            if (playerCoord.x + 1 == enemyCoord.x) {
                return MoveDirections.left;
            }
            else if (playerCoord.x - 1 == enemyCoord.x) {
                return MoveDirections.right;
            }
        }
        else if (playerCoord.x == enemyCoord.x) {
            if (playerCoord.y + 1 == enemyCoord.y) {
                return MoveDirections.up;
            }
            else if (playerCoord.y - 1 == enemyCoord.y) {
                return MoveDirections.down;
            }
        }

        return MoveDirections.none;
    }

    protected virtual void MoveEnemy() {
        var emptySpaces = GetAdjacentEmptySpaces();
        if (emptySpaces.Count == 0) return;

        var player = Managers._turn.Player;

        emptySpaces.Sort((s1, s2) => player.ManhattanDistance((int)s1.x, (int)s1.y) - player.ManhattanDistance((int)s2.x, (int)s2.y));
        Vector2 selectedSpace = emptySpaces[0];
        
        if (selectedSpace == Vector2.up)
            MoveCharacter(MoveDirections.down);
        if (selectedSpace == Vector2.down)
            MoveCharacter(MoveDirections.up);
        if (selectedSpace == Vector2.left)
            MoveCharacter(MoveDirections.left);
        if (selectedSpace == Vector2.right)
            MoveCharacter(MoveDirections.right);
        
    }
}
