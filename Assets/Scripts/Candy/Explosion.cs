using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BoardItem
{
    [SerializeField]
    Animator anim;

    public void SetPosition(int x, int y) {
        transform.position = BoardManager.GetCoords(x, y);
        xPos = x;
        yPos = y;
    }

    private void Update() {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0)) {
            Destroy(this.gameObject);
        }
    }
}
