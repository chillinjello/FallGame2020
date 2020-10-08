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


        //try hit
        List<BoardItem> collidableObjects = new List<BoardItem>();
        collidableObjects.AddRange(Managers._enemy.Enemies);
        collidableObjects.AddRange(Managers._enemy.Walls);
        BoardItem collision = collidableObjects.Find(e => e.xPos == newX && e.yPos == newY);
        if (collision != null) {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.Attack(PLAYER_ATTACK);
            }
            else {
                return false;
            }

            AttackAnimation(direction);
            SnapMovement();
            return true;
        }
        else {
            //check if you moved onto candy
            Candy candy = Managers._candy.Candies.Find(c => c.xPos == newX && c.yPos == newY);
            if (candy != null) {
                candy.PickUpCandy();
            }

            SnapMovement();
            SetPosition(newX, newY);
            SetMoveSprite(newX, newY);
            return true;
        }
    }
}
