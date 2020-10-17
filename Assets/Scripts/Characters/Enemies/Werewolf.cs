using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Werewolf : Enemy
{
    bool justAttacked = false;
    bool movingFirstTime = false;

    public override bool Attack(int amount) {
        if (justAttacked == false) {
            return base.Attack(int.MaxValue);
        }
        else {
            stunned = true;
            return base.Attack(amount);
        }
    }

    public override bool EnemyAttack() {
        attacked = false;
        if (base.EnemyAttack()) {
            justAttacked = true;
            return true;
        }
        return false;
    }

    public override void EnemyMove() {
        if (stunned || attacked || lipStunned > 0) {
            stunned = false;
            attacked = false;
            lipStunned = Mathf.Max(0, lipStunned - 1);
            return;
        }

        justAttacked = false;
        movingFirstTime = true;
        base.EnemyMove();
    }
    
    protected override void MoveCharacter() {
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
                if (movingFirstTime) {
                    movingFirstTime = false;
                    if (!EnemyAttack()) {
                        base.EnemyMove();
                    }
                    else {
                        Managers._enemy.EnemyHitSounds();
                    }
                    return;
                }
                movingFromCauldron = false;
                moving = false;
                attacking = false;
                cauldronCurrentTime = 0f;
                moveTime = 0f;
                attackTime = 0f;

                Managers._turn.SortCharacterLayers();
            }
        }
    }
}
