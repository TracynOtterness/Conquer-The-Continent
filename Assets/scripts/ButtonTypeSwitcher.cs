using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTypeSwitcher : MonoBehaviour {
    private bool originalButtonType = true;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject[] buttonArray = new GameObject[4];
    public Sprite[] sprites;

	private void Start()
	{
        buttonArray[0] = button1;
        buttonArray[1] = button2;
        buttonArray[2] = button3;
        buttonArray[3] = button4;
	}
	public void ChangeButtons(){
        int i = 0;
        if (originalButtonType)
        {
            button1.name = "castle button";
            button2.name = "harbor button";
            button3.name = "ship button";
            button4.name = "village button";
            foreach (GameObject b in buttonArray)
            {
                b.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = sprites[i + 4];
                b.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Cost: 20";
                i++;
            }
            originalButtonType = false;
            UIManager.standardUnits = false;
            UIManager.DarkenButtons();
        }
        else{
            button1.name = "Unit Spawner Tier 1";
            button2.name = "Unit Spawner Tier 2";
            button3.name = "Unit Spawner Tier 3";
            button4.name = "Unit Spawner Tier 4";
            foreach (GameObject b in buttonArray)
            {
                b.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = sprites[i];
                b.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Cost: " + (i+1)*10;
                i++;
            }
            originalButtonType = true;
            UIManager.standardUnits = true;
            UIManager.DarkenButtons();
        }
    }
}
