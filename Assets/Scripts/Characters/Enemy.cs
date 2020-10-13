using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    [SerializeField]
    protected int attackDamage = BASE_ATTACK_DAMAGE;
    const int BASE_ATTACK_DAMAGE = 1;

    protected int lipStunned = 0;
    protected bool stunned = false;
    protected bool attacked = false;

    public override bool Attack(int amount) {
        stunned = true;
        return base.Attack(amount);
    }

    public void StunLips() {
        lipStunned = 5;
    }

    protected override void Update() {
        if (!shaking && currentHealth <= 0) {
            Managers._enemy.Enemies.Remove(this);
            //var puff = Instantiate(Managers._enemy.EnemyPuffPrefab).GetComponent<Explosion>();
            //puff.SetPosition(xPos, yPos);
            //Managers._candy.AddExplosions(puff);
            Destroy(this.gameObject);
        }
        base.Update();
    } 

    public virtual bool EnemyAttack() {
        if (stunned || lipStunned > 0) {
            return false;
        }
        MoveDirections playerDirection = CheckIfPlayerIsInRange();
        if (playerDirection != MoveDirections.none) {
            AttackPlayer(playerDirection);
            attacked = true;
            return true;
        }
        return false;
    }

    public virtual void EnemyMove() {
        if (stunned || attacked || lipStunned > 0) {
            stunned = false;
            attacked = false;
            lipStunned = Mathf.Max(0, lipStunned - 1);
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
        SmartMovement();
    }

    protected virtual void DumbMovement() {
        var emptySpaces = GetAdjacentEmptySpaces();
        if (emptySpaces.Count == 0) return;

        var player = Managers._turn.Player;

        emptySpaces.Sort((s1, s2) => player.ManhattanDistance((int)s1.x, (int)s1.y) - player.ManhattanDistance((int)s2.x, (int)s2.y));
        Vector2 selectedSpace = emptySpaces[0];

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

    protected virtual void SmartMovement() {
        List<Vector2> emptySpaces = FindEmptySpaces();
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
            List<Vector2> emptyAdjacentSpaces = GetAdjacentEmptySpaces((int)currentCoord.x, (int)currentCoord.y, false);
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

    protected virtual void RandomMovement() {
        var emptySpaces = GetAdjacentEmptySpaces();
        if (emptySpaces.Count == 0) return;

        Vector2 selectedSpace = emptySpaces[Random.Range(0,emptySpaces.Count)];

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


