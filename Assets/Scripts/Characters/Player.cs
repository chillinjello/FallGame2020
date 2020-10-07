using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    bool invincible = false;

    [SerializeField]
    Vector2 StartingPos = new Vector2(2, 2);

    const int PLAYER_ATTACK = 1;

    protected override void Awake() {
        base.Awake();

        int startingX = Random.Range(2, 4);
        int startingY = Random.Range(2, 4);
        MoveCharacter(startingX, startingY);
    }

    public bool PlayerTurn(MoveDirections direction) {
        return MoveCharacter(direction);
    }

    public override void Attack(int amount) {
        if (!invincible) {
            base.Attack(amount);
        }
    }

    public override bool MoveCharacter(MoveDirections direction) {
        int newX = xPos;
        int newY = yPos;
        switch (direction) {
            case MoveDirections.up:
                newY -= 1;
                break;
            case MoveDirections.down:
                newY += 1;
                break;
            case MoveDirections.left:
                newX -= 1;
                break;
            case MoveDirections.right:
                newX += 1;
                break;
            default:
                return false;
        }

        if (!BoardManager.CheckValidCoord(newX, newY)) {
            return false;
        }

        SnapMovement();

        //try hit
        var Enemies = Managers._enemy.Enemies;
        var enemy = Enemies.Find(e => e.xPos == newX && e.yPos == newY);
        if (enemy != null) {
            enemy.Attack(PLAYER_ATTACK);
            AttackAnimation(direction);
            return true;
        }

        SetPosition(newX, newY);
        SetMoveSprite(newX, newY);
        return true;
    }
}
