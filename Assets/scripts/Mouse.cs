using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour {
	public bool hasPerson;
	public Vector3 mousePos;
	public Person person;
	public GameObject personObject;
	public GameObject hexObject;
	void Update() {
		mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1);
		mousePos = Camera.main.ScreenToWorldPoint (mousePos);
		this.transform.position = mousePos;
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.tag.Equals ("person") && Input.GetMouseButtonDown (0)) {
			person = col.GetComponent<Person> ();
			personObject = col.gameObject;
			if (!person.hover) {
				if (hasPerson == false) {
					personObject.GetComponent<CircleCollider2D> ().radius = .01f;
					Invoke ("PickedUpBuffer", .1f);
					personObject = col.gameObject;
					person.hover = true;
				}
			}
		}
	}
	void PickedUpBuffer(){
		hasPerson = true;
	}
}