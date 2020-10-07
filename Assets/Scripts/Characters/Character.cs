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
    float moveSpeed = 0.4f;
    float moveTime = 0f;
    public bool moving = false;
    //Attacking
    float attackSpeed = 0.2f;
    float attackSizeMultiplier = 0.2f;
    float attackTime = 0f;
    public bool attacking = false;
    //Both moving and attacking
    public bool movingOrAttacking { get { return moving || attacking; } }
    Vector3 startPosition = Vector3.zero;
    Vector3 movePosition = Vector3.zero;
    public void SnapMovement() {
        if (!moving && !attacking) return;
        moving = false;
        attacking = false;
        moveTime = 0f;
        attackTime = 0f;
        transform.position = movePosition;
    }

    //Character shake
    const int DEFAULT_SHAKE_AMOUNT = 7;
    const float SHAKE_MULTIPLIER = 0.01f;
    const float TIME_BETWEEN_SHAKES = 0.03f;
    float shakeTime = TIME_BETWEEN_SHAKES;
    int shakeAmount;

    //Health
    [SerializeField]
    const int STARTING_HEALTH = 3;
    public int currentHealth = STARTING_HEALTH;
    public bool isAlive() { return currentHealth > 0; }
    public virtual void Attack(int amount) {
        currentHealth -= amount;
        CharacterShake();
    }
    //Health Sprites
    [SerializeField]
    List<Sprite> HealthSprites = new List<Sprite>();
    [SerializeField]
    SpriteRenderer HealthSprite;



    protected virtual void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        MoveCharacter();
        ShakeCharacter();
        SetHealthSprite();
    }

    private void MoveCharacter() {
        if (movingOrAttacking) {
            float speed;
            float time;
            if (attacking) {
                attackTime += Time.deltaTime;
                speed = attackSpeed;
                time = attackTime;
            }
            else {
                moveTime += Time.deltaTime;
                speed = moveSpeed;
                time = moveTime;
            }

            Vector3 pos = new Vector3();
            pos.x = (Mathf.Lerp(startPosition.x, movePosition.x, time / speed));
            pos.y = (Mathf.Lerp(startPosition.y, movePosition.y, time / speed));
            pos.z = transform.position.z;

            transform.position = pos;

            if (transform.position.x == movePosition.x && transform.position.y == movePosition.y) {
                moving = false;
                attacking = false;
                moveTime = 0f;
                attackTime = 0f;
            }
        }
    }

    private void SetHealthSprite() {
        if (currentHealth <= 0) {
            HealthSprite.sprite = HealthSprites[0];
        }
        else if (currentHealth >= 10) {
            HealthSprite.sprite = HealthSprites[10];
        }
        else {
            HealthSprite.sprite = HealthSprites[currentHealth];
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

    protected void AttackAnimation(MoveDirections direction) {
        Vector2 attackSquarePosition = Vector2.zero;
        switch(direction) {
            case MoveDirections.up:
                attackSquarePosition = BoardManager.GetCharacterPosition(sprite.sprite, xPos, yPos - 1);
                break;
            case MoveDirections.down:
                attackSquarePosition = BoardManager.GetCharacterPosition(sprite.sprite, xPos, yPos + 1);
                break;
            case MoveDirections.right:
                attackSquarePosition = BoardManager.GetCharacterPosition(sprite.sprite, xPos + 1, yPos);
                break;
            case MoveDirections.left:
                attackSquarePosition = BoardManager.GetCharacterPosition(sprite.sprite, xPos - 1, yPos);
                break;
        }

        Vector3 differenceVector = new Vector3(attackSizeMultiplier * (transform.position.x - attackSquarePosition.x), attackSizeMultiplier * (transform.position.y - attackSquarePosition.y), 0);

        movePosition = transform.position;
        startPosition = transform.position - differenceVector;
        transform.position = startPosition;
        attacking = true;
        attackTime = 0;
    }

    protected void SetSprite(int x, int y) {
        Vector2 spritePos = BoardManager.GetCharacterPosition(sprite.sprite, x, y);
        transform.position = new Vector3(spritePos.x, spritePos.y, transform.position.z);
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

    protected List<Character> GetAdjacentCharacters() {
        List<Character> adjacentCharacters = new List<Character>();
        adjacentCharacters.AddRange(Managers._enemy.Enemies);
        adjacentCharacters.Add(Managers._turn.Player);

        adjacentCharacters = adjacentCharacters.FindAll(c => {
            if (c.xPos == xPos) {
                if (c.yPos == yPos + 1 || c.yPos == yPos - 1)
                    return true;
            }
            if (c.yPos == yPos) {
                if (c.xPos == xPos + 1 || c.xPos == xPos - 1)
                    return true;
            }
            return false;
        });

        return adjacentCharacters;
    }

    protected List<Vector2> GetAdjacentEmptySpaces() {
        List<Vector2> emptySpace = new List<Vector2>();

        emptySpace.Add(new Vector2(xPos + 1, yPos));
        emptySpace.Add(new Vector2(xPos - 1, yPos));
        emptySpace.Add(new Vector2(xPos, yPos + 1));
        emptySpace.Add(new Vector2(xPos, yPos - 1));

        var characterSpaces = GetAdjacentCharacters();
        emptySpace = emptySpace.FindAll(e => {
            return -1 == characterSpaces.FindIndex(c => e.x == c.xPos && e.y == c.yPos) && BoardManager.CheckValidCoord((int)e.x, (int)e.y) ;
        });

        return emptySpace;
    }

    protected MoveDirections GetDirectionFromCoords(int xi, int yi, int xf, int yf) {
        if (xi == xf) {
            if (yi == yf + 1) {
                return MoveDirections.up;
            }
            if (yi == yf - 1) {
                return MoveDirections.down;
            }
        }
        if (yi == yf) {
            if (xi == xf + 1) {
                return MoveDirections.left;
            }
            if (xi == xf - 1) {
                return MoveDirections.right;
            }
        }
        return MoveDirections.none;
    }

    public int ManhattanDistance(int x, int y) {
        return Mathf.Abs(xPos - x) + Mathf.Abs(yPos - y);
    }


    public virtual void CharacterShake() {
        shakeAmount = DEFAULT_SHAKE_AMOUNT;
    }
    
    protected virtual void ShakeCharacter() {
        if (shakeAmount <= 0)
            return;
        
        if (TIME_BETWEEN_SHAKES > shakeTime) {
            shakeTime += Time.deltaTime;
            return;
        }
        else {
            shakeTime = 0;
        }

        if (shakeAmount == 1) {
            SetSprite(xPos, yPos);
            shakeTime = TIME_BETWEEN_SHAKES;
            shakeAmount = 0;
            return;
        }

        shakeAmount--;

        float shakeAngle = Random.Range(0f, 1f) * Mathf.PI * 2;
        float shakeX = BoardManager.FixToPixel((Mathf.Cos(shakeAngle) * shakeAmount) * SHAKE_MULTIPLIER);
        float shakeY = BoardManager.FixToPixel((Mathf.Sin(shakeAngle) * shakeAmount) * SHAKE_MULTIPLIER);

        if (!movingOrAttacking) {
            SetSprite(xPos, yPos);
        }
        transform.position += new Vector3(shakeX, shakeY);
    }
}
