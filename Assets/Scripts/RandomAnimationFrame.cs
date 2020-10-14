using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationFrame : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim != null) {
            anim.Play(0, -1, Random.Range(0f, 1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anim == null) {
            anim = GetComponent<Animator>();
            if (anim != null) {
                anim.Play(0, -1, Random.Range(0f, 1f));
            }
            else {
                Debug.Log("cant find animator to play");
            }
        }
    }
}
