using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSizeButton : MonoBehaviour
{
    public Button[] mapButtons;
    public Button[] controlButtons;
    public void ChangeMapSize(int size)
    {
        Instantiator.size = size;
        foreach (Button b in mapButtons)
        {
            if (!b.interactable)
            {
                b.interactable = true;
            }
        }
        mapButtons[size - 1].interactable = false;
    }
    public void ChangeControlStyle(bool hover)
    {
        if (hover)
        {
            controlButtons[1].interactable = true;
            controlButtons[0].interactable = false;
            UIManager.hoverMode = true;
        }
        else
        {
            controlButtons[0].interactable = true;
            controlButtons[1].interactable = false;
            UIManager.hoverMode = false;
        }
    }
}