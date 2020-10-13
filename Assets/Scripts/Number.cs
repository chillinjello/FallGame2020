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

    [SerializeField]
    GameObject PreviousComma;

    private void Start() {
        renderer = GetComponent<SpriteRenderer>();
        if (!isComma)
            renderer.sprite = numberSprites[currentNumber];
        else
            renderer.sprite = commaSprite;
    }

    public void SetNumber(string n) {

        if (n == "none") {
            currentNumber = 0;
            if (renderer != null) renderer.sprite = numberSprites[0];
            this.gameObject.SetActive(false);
            if (PreviousComma != null)
                PreviousComma.SetActive(false);
            if (moreSignificantNumber != null)
                moreSignificantNumber.SetNumber("none");
            return;
        }

        if (PreviousComma != null)
            PreviousComma.SetActive(true);
        this.gameObject.SetActive(true);

        if (n.Length == 0) return;
        
        char number = n[n.Length - 1];
        if (n.Length > 1 && moreSignificantNumber != null) {
            moreSignificantNumber.SetNumber(n.Remove(n.Length - 1));
        }
        else if (moreSignificantNumber != null) {
            moreSignificantNumber.SetNumber("none");
        }

        currentNumber = (int)char.GetNumericValue(number);
        if (currentNumber >= 0 && currentNumber < 10)
            if (renderer != null) renderer.sprite = numberSprites[currentNumber];
    }

    public void SetNumber(int i) {
        SetNumber(i.ToString());
    }
    
}
