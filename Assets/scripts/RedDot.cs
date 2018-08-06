using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDot : MonoBehaviour {
    public float time = .5f;
    Sprite sprite;
    public Person person;
    bool flash = true;

	// Use this for initialization
	void Start () {
        sprite = Resources.Load<Sprite>("Red Dot");
	}
	
	// Update is called once per frame
	void Update () {
        if (person.hasUsedMove == true){
            flash = false;
            this.GetComponent<SpriteRenderer>().sprite = null;
        }
        if (flash){
            time -= Time.deltaTime;
            if (time <= 0)
            {
                if (this.GetComponent<SpriteRenderer>().sprite == null)
                {
                    this.GetComponent<SpriteRenderer>().sprite = sprite;
                }
                else this.GetComponent<SpriteRenderer>().sprite = null;
                time = .5f;
            }
        }
	}
}
