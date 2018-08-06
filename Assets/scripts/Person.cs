using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour {
	public static bool hasPerson = false;
	public static Vector3 mousePos;
	public static Person person;
	public static GameObject personObject;
	public GameObject hexObject;
    bool pickedUpIsValid;
    private GameObject parent;
    private bool clickedHexagonHasCountry;

	void Update() {
		mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1);
		mousePos = Camera.main.ScreenToWorldPoint (mousePos);
		this.transform.position = mousePos;
	}
	void OnTriggerStay2D(Collider2D col){
        try
        {
            parent = col.gameObject.transform.parent.gameObject;
            clickedHexagonHasCountry = true;
        }
        catch (Exception) { clickedHexagonHasCountry = false; }
		if (col.gameObject.tag.Equals ("person") && Input.GetMouseButtonDown (0)) {
            if(!hasPerson){
                person = col.GetComponent<Person>();
                personObject = col.gameObject;
                pickedUpIsValid = PickedUpIsValid();
                if (pickedUpIsValid) { PickUpUnit(); }  
            }
		}
        else if (col.gameObject.tag.Equals("capital") && Input.GetMouseButtonDown(0) && UIManager.turnNumber == col.gameObject.transform.parent.GetComponent<Hexagon>().nationNum - 2 && UIManager.selectedCountry != parent.transform.parent.gameObject){
            if (!EventSystem.current.IsPointerOverGameObject()) UIManager.SetCurrentCountry(parent.transform.parent.gameObject);
        }
	}
	void PickedUpBuffer(){
        hasPerson = true;
        //maybe put something here using Time.deltaTime to preven the thing where it picks up a unit and places it back down immedietly
	}
    public void PickUpUnit(){
        if (!person.hover)
        {
            if (hasPerson == false && person.hasUsedMove == false)
            {
                person.redDot.layer = 9;
                personObject.GetComponent<CircleCollider2D>().radius = .0036f;
                Invoke("PickedUpBuffer", .1f);
                person.hover = true;
            }
        }
    }
    public bool PickedUpIsValid(){
        //do the isvalid method for unit here
        if (UIManager.selectedCountry == person.country){
            return true;  
        }
        return false;
    }
}
