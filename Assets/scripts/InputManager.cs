using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
    Vector3 mousePos;
    GameObject mouse;
    UIManager uiManager;
    public RawImage minimap;
    ButtonTypeSwitcher bts;
	// Update is called once per frame
	private void Start()
	{
        mouse = GameObject.Find("Mouse");
        bts = GameObject.Find("Unit type switch button").GetComponent<ButtonTypeSwitcher>();
	}
	void Update () {
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mouse.transform.position = mousePos;

        if (Input.GetKeyDown(KeyCode.C)) { TestOnCommand(); }

        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject r in UIManager.redDots)
            {
                if (r.layer == 9)
                {
                    r.layer = 0;
                    UIManager.toggled = true;
                }
                else
                {
                    r.layer = 9;
                    UIManager.toggled = false;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            minimap.enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.M))
        {
            minimap.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.A)){
            UIManager.SpawnUnits("1");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UIManager.SpawnUnits("2");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UIManager.SpawnUnits("3");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            UIManager.SpawnUnits("4");
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            bts.ChangeButtons();
        }

        if(Input.GetKeyDown(KeyCode.N)){
            FindObjectOfType<UIManager>().NextTurn();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            Screen.fullScreen = !Screen.fullScreen;
        }
	}
    void TestOnCommand()
    {
        print(Mouse.hasUnit);
        print(Mouse.person);
    }
}
