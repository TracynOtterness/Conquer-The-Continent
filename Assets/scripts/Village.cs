using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour {

    public bool hover = false;
	void Update () {
        if(hover == true && UIManager.hoverMode){
            this.transform.position = Mouse.mousePos;
        }
	}
}
