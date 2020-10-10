using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BoardItem
{
    [SerializeField]
    Sprite frame1;
    [SerializeField]
    Sprite frame2;

    [SerializeField]
    GameObject explosionPrefab;

    SpriteRenderer renderer;

    const int STARTING_TICK = 3;
    int currentTick = STARTING_TICK;

    public void SetPosition(int x, int y) {
        transform.position = BoardManager.GetCoords(x, y);
        xPos = x;
        yPos = y;
    }

    private void Awake() {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Tick() {
        currentTick--;

        if (currentTick == 1) {
            renderer.sprite = frame2;
        }
        else if (currentTick == 0) {
            Explode();
        }
    }

    public void Explode() {
        List<Explosion> explosions = new List<Explosion>();

        List<Character> characters = new List<Character>();
        characters.AddRange(Managers._enemy.Enemies);
        characters.Add(Managers._turn.Player);

        Explosion explosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
        explosion.SetPosition(xPos, yPos);
        explosions.Add(explosion);
        characters.ForEach(c => {
            if (c.xPos == xPos && yPos == c.yPos) {
                c.Attack(3);
            }
        });

        if (BoardManager.CheckValidCoord(xPos + 1,yPos)) {
            characters.ForEach(c => {
                if (c.xPos == xPos + 1 && yPos == c.yPos) {
                    c.Attack(3);
                }
            });
            explosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
            explosion.SetPosition(xPos + 1, yPos);
            explosions.Add(explosion);
        }
        if (BoardManager.CheckValidCoord(xPos - 1, yPos)) {
            characters.ForEach(c => {
                if (c.xPos == xPos - 1 && yPos == c.yPos) {
                    c.Attack(3);
                }
            });
            explosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
            explosion.SetPosition(xPos - 1, yPos);
            explosions.Add(explosion);
        }
        if (BoardManager.CheckValidCoord(xPos, yPos + 1)) {
            characters.ForEach(c => {
                if (c.xPos == xPos && yPos + 1 == c.yPos) {
                    c.Attack(3);
                }
            });
            explosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
            explosion.SetPosition(xPos, yPos + 1);
            explosions.Add(explosion);
        }
        if (BoardManager.CheckValidCoord(xPos, yPos - 1)) {
            characters.ForEach(c => {
                if (c.xPos == xPos && yPos - 1 == c.yPos) {
                    c.Attack(3);
                }
            });
            explosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
            explosion.SetPosition(xPos, yPos - 1);
            explosions.Add(explosion);
        }

        Managers._candy.RemoveBomb(this);

        Managers._candy.AddExplosions(explosions);
        
        Destroy(this.gameObject);
    }
}
