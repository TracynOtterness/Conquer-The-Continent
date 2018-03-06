using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMovement : MonoBehaviour {

	public Mouse mouse;
	private int nationNum;
	public GameObject personObject;
	private bool valid = true;
	private int guardedBy;
	private Person person;
		void OnTriggerStay2D(Collider2D col) {
		mouse = col.GetComponent<Mouse> ();
		if (col.gameObject.tag.Equals ("mouse") && mouse.hasPerson == true && Input.GetMouseButtonUp (0) == true) {
			personObject = mouse.personObject;
			person = personObject.GetComponent<Person> ();
			if (person.hover) {
				if (valid) {
					mouse.hasPerson = false; 
					guardedBy = person.tier;
					if (this.nationNum != person.nationNum) {
						this.nationNum = person.nationNum;
					}
				}
				person.Place ();
			} 
		}
	}
}

