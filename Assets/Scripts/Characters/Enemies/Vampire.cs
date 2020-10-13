using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : Enemy
{
    [SerializeField]
    Sprite VampireSprite;
    [SerializeField]
    Sprite BatSprite;

    bool isBat = false;
    

    protected override void SetPosition(int x, int y) {
        base.SetPosition(x, y);

        if (Managers._enemy.Walls.FindIndex(w => w.xPos == x && w.yPos == y) != -1) {
            sprite.sprite = BatSprite;
            sprite.sortingOrder = 1;
            isBat = true;
        }
        else {
            sprite.sprite = VampireSprite;
            sprite.sortingOrder = 0;
            isBat = false;
        }
    }

    public override bool EnemyAttack() {
        if (base.EnemyAttack()) {
            currentHealth++;
            return true;
        }
        return false;
    }

    protected override void MoveEnemy() {
        SmartMovement();
    }

    protected override void SmartMovement() {
        List<Vector2> emptySpaces = FindEmptySpaces(false, false, false);
        Player player = Managers._turn.Player;
        Vector2 playerCoord = player.GetPos();

        List<Vector2> openSet = new List<Vector2>();
        openSet.Add(GetPos());

        List<(Vector2, Vector2)> cameFrom = new List<(Vector2, Vector2)>();

        List<Vector2> visited = new List<Vector2>();

        while (openSet.Count > 0) {
            openSet.Sort((s1, s2) => player.ManhattanDistance((int)s1.x, (int)s1.y) - player.ManhattanDistance((int)s2.x, (int)s2.y));

            var currentCoord = openSet[0];
            if (currentCoord == playerCoord) {
                //reconstruct path
                var lastCoord = cameFrom.Find(c => c.Item2 == currentCoord);
                while (lastCoord.Item1 != GetPos()) {
                    lastCoord = cameFrom.Find(c => c.Item2 == lastCoord.Item1);
                }
                MoveCharacter(GetDirectionFromCoords(xPos, yPos, (int)lastCoord.Item2.x, (int)lastCoord.Item2.y));
                return;
            }

            openSet.Remove(currentCoord);
            List<Vector2> emptyAdjacentSpaces = GetAdjacentEmptySpaces((int)currentCoord.x, (int)currentCoord.y, false, false);
            emptyAdjacentSpaces.ForEach(s => {
                if (visited.FindIndex(v => v == s) == -1) {
                    visited.Add(s);
                    openSet.Add(s);
                    cameFrom.Add((currentCoord, s));
                }
            });
        }

        DumbMovement();
    }

    protected override void DumbMovement() {
        var emptySpaces = GetAdjacentEmptySpaces(false, false);
        if (emptySpaces.Count == 0) return;

        var player = Managers._turn.Player;

        emptySpaces.Sort((s1, s2) => player.ManhattanDistance((int)s1.x, (int)s1.y) - player.ManhattanDistance((int)s2.x, (int)s2.y));
        Vector2 selectedSpace = emptySpaces[0];

        if (selectedSpace == player.GetPos()) return;
        Vector2 direction = selectedSpace - new Vector2(xPos, yPos);
        if (direction == Vector2.up)
            MoveCharacter(MoveDirections.down);
        if (direction == Vector2.down)
            MoveCharacter(MoveDirections.up);
        if (direction == Vector2.left)
            MoveCharacter(MoveDirections.left);
        if (direction == Vector2.right)
            MoveCharacter(MoveDirections.right);
    }
}
