using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hexagon : MonoBehaviour {
	public Instantiator instantiator;
	public int HexPosX;
	public int HexPosY;
    public int NationNum;
	private float b = .2f;
	public bool isLand = false;
	private Sprite[] spriteArray;

	void Awake () {
		spriteArray = Resources.LoadAll<Sprite>("sprites");
		instantiator = GameObject.FindObjectOfType<Instantiator> ();
		HexPosX = instantiator.HexPosX;
		HexPosY = instantiator.HexPosY;
		string HexString = "Hexagon " + HexPosX + " , " + HexPosY;
		this.name = HexString;
		if (HexPosX % 2 == 1) {
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + b, this.transform.position.z);
		}
	}
	public void CreateLand() {
		this.isLand = true;
        NationNum = Random.Range(3,8);
		this.GetComponent<SpriteRenderer> ().sprite = spriteArray [NationNum];
		instantiator.totalHexes++;
}
}