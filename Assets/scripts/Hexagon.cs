using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hexagon : MonoBehaviour {
	public Instantiator instantiator;
	public int HexPosX;
	public int HexPosY;
    public int nationNum;
	private float b = .2f;
	public bool isLand = false;
    public bool isCountry = false;
	private Sprite[] spriteArray;
    public GameObject[] adj = new GameObject[6];

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
    void Start(){

            adj[0] = GameObject.Find("Hexagon " + HexPosX + " , " + (HexPosY + 1));
            adj[3] = GameObject.Find("Hexagon " + HexPosX + " , " + (HexPosY - 1));
             if (HexPosX % 2 == 0)
        {
            adj[1] = GameObject.Find("Hexagon " + (HexPosX + 1) + " , " + HexPosY);
            adj[2] = GameObject.Find("Hexagon " + (HexPosX + 1) + " , " + (HexPosY - 1));
            adj[4] = GameObject.Find("Hexagon " + (HexPosX - 1) + " , " + (HexPosY - 1));
            adj[5] = GameObject.Find("Hexagon " + (HexPosX - 1) + " , " + HexPosY);
        }
        else
        {
            adj[1] = GameObject.Find("Hexagon " + (HexPosX + 1) + " , " + (HexPosY + 1));
            adj[2] = GameObject.Find("Hexagon " + (HexPosX + 1) + " , " + HexPosY);
            adj[4] = GameObject.Find("Hexagon " + (HexPosX - 1) + " , " + HexPosY);
            adj[5] = GameObject.Find("Hexagon " + (HexPosX - 1) + " , " + (HexPosY + 1));
        }
        }

	public void CreateLand() {
		this.isLand = true;
        nationNum = Random.Range(3,8);
		this.GetComponent<SpriteRenderer> ().sprite = spriteArray [nationNum];
		instantiator.totalHexes++;
}
}