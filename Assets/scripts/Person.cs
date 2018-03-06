using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public Mouse mouse;
	public int tier;
	public int nationNum;
	private Vector3 pos;
	public bool hover;
	private GameObject hex;
	void Update(){
		if (hover)
			this.transform.position = mouse.mousePos;
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.tag.Equals ("hex")) {
			hex = col.gameObject;
		}
		if (col.gameObject.tag.Equals ("mouse")) {
			mouse = col.GetComponent<Mouse> ();
		}
	}
	public void Place() {
		this.GetComponent<CircleCollider2D> ().radius = 3;
		pos = hex.transform.position;
		this.transform.position = pos;
		hover = false;
		mouse.hasPerson = false;
	}
}