using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;

    public int xPos = -1;
    public int yPos = -1;
    public Vector2 GetPos() { return new Vector2(xPos, yPos); }

    public enum MoveDirections {
        up,
        down,
        left,
        right,
        none
    }

    //Moving
    [SerializeField]
    float moveSpeed = 0.2f;
    public bool moving = false;
    float moveTime = 0f;
    Vector3 startPosition = Vector3.zero;
    Vector3 movePosition = Vector3.zero;
    public void SnapMovement() {
        if (!moving) return;
        moving = true;
        moveTime = 0f;
        transform.position = movePosition;
    }

    //Health
    [SerializeField]
    const int STARTING_HEALTH = 3;
    public int currentHealth = STARTING_HEALTH;
    public bool isAlive() { return currentHealth > 0; }
    public virtual void Attack(int amount) { currentHealth -= amount; }

    protected virtual void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (moving) {
            moveTime += Time.deltaTime;
            Vector3 pos = new Vector3();
            pos.x = (Mathf.Lerp(startPosition.x, movePosition.x, moveTime / moveSpeed));
            pos.y = (Mathf.Lerp(startPosition.y, movePosition.y, moveTime / moveSpeed));

            
            transform.position = pos;

            if (transform.position.x == movePosition.x && transform.position.y == movePosition.y) {
                moving = false;
                moveTime = 0f;
            }
        }
    }

    protected void SetPosition(int x, int y) {
        xPos = x;
        yPos = y;

    }

    protected void SetMoveSprite(int x, int y) {
        moveTime = 0f;
        moving = true;
        startPosition = transform.position;
        movePosition = BoardManager.GetCharacterPosition(sprite.sprite, x, y);
    }

    protected void SetSprite(int x, int y) {
        transform.position = BoardManager.GetCharacterPosition(sprite.sprite, x, y);
    }

    public bool MoveCharacter(int x, int y) {
        if (!BoardManager.CheckValidCoord(x, y))
            return false;

        //check if there's anything already on the square

        SetPosition(x, y);
        SetSprite(x, y);
        return true;
    }

    public virtual bool MoveCharacter(MoveDirections direction) {
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

        if (Managers._enemy.Enemies.Find(e => newX == e.xPos && e.yPos == newY) != null)
            return false;

        SetPosition(newX, newY);
        SetMoveSprite(newX, newY);
        return true;
    }
}
