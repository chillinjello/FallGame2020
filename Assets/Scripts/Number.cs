using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{
    [SerializeField]
    List<Sprite> numberSprites;
    [SerializeField]
    Sprite commaSprite;

    [SerializeField]
    bool isComma = false;

    SpriteRenderer renderer;

    int currentNumber = 0;

    [SerializeField]
    Number moreSignificantNumber = null;

    private void Awake() {
        renderer = GetComponent<SpriteRenderer>();
        if (!isComma)
            renderer.sprite = numberSprites[currentNumber];
        else
            renderer.sprite = commaSprite;
    }

    public void SetNumber(string n) {
        if (n.Length == 0) return;
        char number = n[n.Length - 1];
        if (n.Length > 1 && moreSignificantNumber != null) {
            moreSignificantNumber.SetNumber(n.Remove(n.Length - 1));
        }

        currentNumber = (int)char.GetNumericValue(number);
        renderer.sprite = numberSprites[currentNumber];
    }

    public void SetNumber(int i) {
        SetNumber(i.ToString());
    }
    
}
