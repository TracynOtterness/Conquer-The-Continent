using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{

    Sprite[] sprites;
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Hexes");
    }

    public void ChangeSprite(int a)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = sprites[a];
    }
}