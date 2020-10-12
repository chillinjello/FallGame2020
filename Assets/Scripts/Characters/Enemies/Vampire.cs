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
