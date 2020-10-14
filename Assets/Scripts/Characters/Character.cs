using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BoardItem
{
    [SerializeField]
    protected SpriteRenderer sprite;
    protected void FlipSprite(bool f) { sprite.flipX = f; }
    

    //Moving
    [SerializeField]
    protected float moveSpeed = 0.4f;
    protected float moveTime = 0f;
    public bool moving = false;
    //Attacking
    protected float attackSpeed = 0.2f;
    float attackSizeMultiplier = 0.2f;
    protected float attackTime = 0f;
    public bool attacking = false;
    //Both moving and attacking
    public bool movingOrAttacking { get { return moving || attacking || movingFromCauldron; } }
    protected Vector3 startPosition = Vector3.zero;
    protected Vector3 movePosition = Vector3.zero;
    //Move from witch cauldron
    public bool movingFromCauldron;
    //float cauldron_w = 0;
    //float cauldron_phi = 0;
    Vector2 STARTING_POS = new Vector2(7.5f, 0.5f);
    //const float MAX_HEIGHT = 4f;
    protected const float CAULDRON_TIME = .4f;
    protected float cauldronCurrentTime = 0;
    //snap movement function
    public virtual void SnapMovement() {
        if (!moving && !attacking) return;
        moving = false;
        attacking = false;
        moveTime = 0f;
        attackTime = 0f;
        transform.position = movePosition;
    }

    //Character shake
    const int DEFAULT_SHAKE_AMOUNT = 7;
    const int DEAD_SHAKE_AMOUNT = 14;
    const float SHAKE_MULTIPLIER = 0.01f;
    const float DEAD_SHAKE_MULTIPLIER = 0.015f;
    const float TIME_BETWEEN_SHAKES = 0.03f;
    float shakeTime = TIME_BETWEEN_SHAKES;
    int shakeAmount;
    public bool shaking { get { return shakeAmount > 0; } }

    //Health
    [SerializeField]
    const int STARTING_HEALTH = 3;
    public int currentHealth = STARTING_HEALTH;
    public bool isAlive() { return currentHealth > 0; }
    public virtual bool Attack(int amount) {
        currentHealth -= amount;
        CharacterShake(currentHealth <= 0);
        if (currentHealth <= 0)
            return true;
        return false;
    }
    //Health Sprites
    [SerializeField]
    List<Sprite> HealthSprites = new List<Sprite>();
    [SerializeField]
    SpriteRenderer HealthSprite;



    protected virtual void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() {
        MoveCharacter();
        ShakeCharacter();
        SetHealthSprite();
    }

    protected virtual void MoveCharacter() {
        if (movingOrAttacking) {

            Vector3 pos = new Vector3();
            if (movingFromCauldron) {
                cauldronCurrentTime += Time.deltaTime;
                pos = Vector3.Slerp(startPosition, movePosition, cauldronCurrentTime / CAULDRON_TIME);
                if (cauldronCurrentTime > CAULDRON_TIME) {
                    pos.x = movePosition.x;
                    pos.y = movePosition.y;
                }
            }
            else if (moving || attacking) {
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
                pos.x = (Mathf.Lerp(startPosition.x, movePosition.x, time / speed));
                pos.y = (Mathf.Lerp(startPosition.y, movePosition.y, time / speed));
                pos.z = transform.position.z;
            }


            transform.position = pos;

            if (transform.position.x == movePosition.x && transform.position.y == movePosition.y) {
                moving = false;
                attacking = false;
                movingFromCauldron = false;
                cauldronCurrentTime = 0f;
                moveTime = 0f;
                attackTime = 0f;

                Managers._turn.SortCharacterLayers();
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

    protected virtual void SetPosition(int x, int y) {
        if (x > xPos) {
            FlipSprite(false);
        }
        else if (x < xPos) {
            FlipSprite(true); 
        }

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
                FlipSprite(false);
                break;
            case MoveDirections.left:
                attackSquarePosition = BoardManager.GetCharacterPosition(sprite.sprite, xPos - 1, yPos);
                FlipSprite(true);
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

    public virtual bool MoveCharacterSmooth(int x, int y) {
        if (!BoardManager.CheckValidCoord(x, y))
            return false;

        //check if there's anything already on the square

        SetPosition(x, y);
        SetMoveSprite(x, y);
        return true;
    }

    public virtual void CharacterShake(bool dead = false) {
        if (dead) {
            shakeAmount = DEAD_SHAKE_AMOUNT;
        }
        else {
            shakeAmount = DEFAULT_SHAKE_AMOUNT;
        }
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
        float shakeX = BoardManager.FixToPixel((Mathf.Cos(shakeAngle) * shakeAmount) * (currentHealth <= 0 ? DEAD_SHAKE_MULTIPLIER: SHAKE_MULTIPLIER));
        float shakeY = BoardManager.FixToPixel((Mathf.Sin(shakeAngle) * shakeAmount) * (currentHealth <= 0 ? DEAD_SHAKE_MULTIPLIER : SHAKE_MULTIPLIER));

        if (!movingOrAttacking) {
            SetSprite(xPos, yPos);
        }
        transform.position += new Vector3(shakeX, shakeY);
    }

    public static List<Vector2> GetAdjacentCoords(int x, int y) {
        List<Vector2> adjacentCoords = new List<Vector2>();
        if (BoardManager.CheckValidCoord(x + 1, y)) {
            adjacentCoords.Add(new Vector2(x + 1,y));
        }
        if (BoardManager.CheckValidCoord(x - 1, y)) {
            adjacentCoords.Add(new Vector2(x - 1, y));
        }
        if (BoardManager.CheckValidCoord(x, y + 1)) {
            adjacentCoords.Add(new Vector2(x, y + 1));
        }
        if (BoardManager.CheckValidCoord(x, y - 1)) {
            adjacentCoords.Add(new Vector2(x, y - 1));
        }

        return adjacentCoords;
    }

    public bool SeeIfCharacterCanReachAllSpaces() {
        List<Vector2> allCoords = BoardManager.GetAllCoords();
        List<Wall> walls = Managers._enemy.Walls;

        for (int i = allCoords.Count - 1; i >= 0; i--) {
            if (walls.FindIndex(w => w.xPos == allCoords[i].x && w.yPos == allCoords[i].y) != -1) {
                allCoords.RemoveAt(i);
            }
        }

        Stack<Vector2> coordStack = new Stack<Vector2>();
        List<Vector2> visitedCoords = new List<Vector2>();
        GetAdjacentCoords(xPos,yPos).ForEach(c => {
            if (walls.FindIndex(w => w.xPos == c.x && w.yPos == c.y) == -1) {
                coordStack.Push(c);
                visitedCoords.Add(c);
            }
        });
        while (coordStack.Count > 0) {
            Vector2 currentCoord = coordStack.Pop();
            
            var newCoords = GetAdjacentCoords((int)currentCoord.x, (int)currentCoord.y);
            newCoords.ForEach(n => {
                if (walls.FindIndex(w => w.xPos == n.x && w.yPos == n.y) == -1 && visitedCoords.FindIndex(v => v.x == n.x && v.y == n.y) == -1) {
                    visitedCoords.Add(n);
                    coordStack.Push(n);
                }
            });
        }
        if (allCoords.Count != visitedCoords.Count)
            return false;

        return true;
    }

    public bool SetMoveFromCauldron(int x, int y) {
        if (!BoardManager.CheckValidCoord(x, y))
            return false;

        Vector2 finalPos = BoardManager.GetCharacterPosition(sprite.sprite ,x, y);
        transform.position = STARTING_POS;

        cauldronCurrentTime = 0;
        movingFromCauldron = true;

        startPosition = STARTING_POS;
        movePosition = finalPos;
        
        SetPosition(x, y);

        Managers._enemy.SetWitchSpriteOn();

        return true;
    }
}
