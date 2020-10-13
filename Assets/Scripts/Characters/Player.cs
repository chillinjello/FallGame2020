using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    Sprite batSprite;
    [SerializeField]
    Sprite startingSprite;
    [SerializeField]
    Sprite lipsSprite;
    [SerializeField]
    Sprite lipsFangSprite;
    [SerializeField]
    Sprite lipsWingSprite;
    [SerializeField]
    Sprite lipsPopSprite;
    [SerializeField]
    Sprite lipsFangWingSprite;
    [SerializeField]
    Sprite lipsFangPopSprite;
    [SerializeField]
    Sprite lipsFangWingPopSprite;
    [SerializeField]
    Sprite lipsWingPopSprite;
    [SerializeField]
    Sprite fangSprite;
    [SerializeField]
    Sprite fangWingSprite;
    [SerializeField]
    Sprite fangPopSprite;
    [SerializeField]
    Sprite fangWingPopSprite;
    [SerializeField]
    Sprite wingSprite;
    [SerializeField]
    Sprite wingPopSprite;
    [SerializeField]
    Sprite popSprite;


    [SerializeField]
    bool invincible = false;

    [SerializeField]
    Vector2 StartingPos = new Vector2(2, 2);

    [SerializeField]
    int PLAYER_STARTING_HEALTH = 4;

    const int PLAYER_ATTACK = 1;

    private int stunLips = 0;
    public void AddStunLips () {
        stunLips++;
        SetPlayerSprite();
    }
    private int increasedAttack = 0;
    public void AddIncreasedAttack () {
        increasedAttack++;
        SetPlayerSprite();
    }
    private int batWings = 0;
    private bool batWingsOn = false;
    public void AddBatWings() {
        batWings++;
        SetPlayerSprite();
    }
    private int vampire = 0;
    const int VAMPIRE_AMOUNT = 4;
    public void AddVampire() {
        vampire += VAMPIRE_AMOUNT;
        SetPlayerSprite();
    }
    public void TickVampire() {
        vampire = Mathf.Max(0,vampire-1);
        SetPlayerSprite();
    }

    protected override void Awake() {
        base.Awake();

        currentHealth = PLAYER_STARTING_HEALTH;

        int startingX = Random.Range(2, 4);
        int startingY = Random.Range(2, 4);
        MoveCharacter(startingX, startingY);
    }

    public bool PlayerTurn(MoveDirections direction) {
        return MoveCharacter(direction);
    }

    public override bool Attack(int amount) {
        if (!invincible) {
            return base.Attack(amount);
        }
        return false;
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
            Wall wall = collision.gameObject.GetComponent<Wall>();
            if (enemy != null) {
                if (increasedAttack > 0) {
                    if (enemy.Attack(int.MaxValue)) {
                        Managers._player.AddScore(PlayerStatusManager.LOLLYPOP_KILL_SCORE);
                    }
                    increasedAttack--;
                    SetPlayerSprite();
                }
                else {
                    if (enemy.Attack(PLAYER_ATTACK)) {
                        Managers._player.AddScore(PlayerStatusManager.KILL_ENEMY_SCORE);
                    }
                }
                if (stunLips > 0) {
                    enemy.StunLips();
                    stunLips--;
                    SetPlayerSprite();
                }
                if (vampire > 0) {
                    currentHealth++;
                }
                SnapMovement();
                AttackAnimation(direction);
                return true;
            }
            else if (wall != null) {
                if (batWingsOn || batWings > 0) {
                    batWingsOn = true;
                    SetPlayerSprite();
                    SnapMovement();
                    SetPosition(newX, newY);
                    SetMoveSprite(newX, newY);
                    return true;
                }
            }
            return false;
        }
        else {
            //check if you moved onto candy
            Candy candy = Managers._candy.Candies.Find(c => c.xPos == newX && c.yPos == newY);
            if (candy != null) {
                candy.PickUpCandy();
            }

            if (batWingsOn) {
                batWingsOn = false;
                batWings--;
                SetPlayerSprite();
            }
            SnapMovement();
            SetPosition(newX, newY);
            SetMoveSprite(newX, newY);
            return true;
        }
    }

    private void SetPlayerSprite() {
        sprite.sortingOrder = 0;
        if (batWingsOn) {
            sprite.sprite = batSprite;
            sprite.sortingOrder = 1;
        }

        else if (stunLips > 0) {
            if (vampire > 0) {
                if (batWings > 0) {
                    if (increasedAttack > 0) {
                        sprite.sprite = lipsFangWingPopSprite;
                    }
                    else {
                        sprite.sprite = lipsFangWingSprite;
                    }
                }
                else {
                    if (increasedAttack > 0) {
                        sprite.sprite = lipsFangPopSprite;
                    }
                    else {
                        sprite.sprite = lipsFangSprite;
                    }
                }
            }
            else {
                if (batWings > 0) {
                    if (increasedAttack > 0) {
                        sprite.sprite = lipsWingPopSprite;
                    }
                    else {
                        sprite.sprite = lipsWingSprite;
                    }
                }
                else {
                    if (increasedAttack > 0) {
                        sprite.sprite = lipsPopSprite;
                    }
                    else {
                        sprite.sprite = lipsSprite;
                    }
                }
            }
        }
        else {
            if (vampire > 0) {
                if (batWings > 0) {
                    if (increasedAttack > 0) {
                        sprite.sprite = fangWingPopSprite;
                    }
                    else {
                        sprite.sprite = fangWingSprite;
                    }
                }
                else {
                    if (increasedAttack > 0) {
                        sprite.sprite = fangPopSprite;
                    }
                    else {
                        sprite.sprite = fangSprite;
                    }
                }
            }
            else {
                if (batWings > 0) {
                    if (increasedAttack > 0) {
                        sprite.sprite = wingPopSprite;
                    }
                    else {
                        sprite.sprite = wingSprite;
                    }
                }
                else {
                    if (increasedAttack > 0) {
                        sprite.sprite = popSprite;
                    }
                    else {
                        sprite.sprite = startingSprite;
                    }
                }
            }
        }







        
    }
}
