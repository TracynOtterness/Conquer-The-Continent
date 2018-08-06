using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hexagon : MonoBehaviour {
	public int HexPosX;
	public int HexPosY;
    public int nationNum;
	private float b = .193f;
	public bool isLand = false;
    public bool isCountry = false;
	private Sprite[] spriteArray;
    public GameObject[] adj = new GameObject[6];
    public int guardedBy = 0;
    public bool hasGuard;
    public bool hasCapital;
    public bool hasGrave;
    public GameObject village;
    public GameObject grave;
    public Sprite[]capitalSpriteArray = new Sprite[2];
    public Person guard; 

	void Awake () {
		spriteArray = Resources.LoadAll<Sprite>("Hexes");
		HexPosX = Instantiator.HexPosX;
		HexPosY = Instantiator.HexPosY;
		string HexString = "Hexagon " + HexPosX + " , " + HexPosY;
		this.name = HexString;
		if (HexPosX % 2 == 1) {
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + b, this.transform.position.z);
		}
        EditFlag();
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
		Instantiator.totalHexes++;
}
    public void ChangeSprite(int a){
        this.GetComponent<SpriteRenderer>().sprite = spriteArray[a];
    }
    public void AddCapital(){
        hasCapital = true;
        village = this.gameObject.transform.GetChild(0).gameObject;
        guardedBy = 1;
    }
    public void RemoveCapital(){
        hasCapital = false;
        Destroy(village);
    }
    public void EditFlag(){
        if (village == null) return;
        if (transform.parent.GetComponent<Country>().food >= 10){
            village.GetComponent<SpriteRenderer>().sprite = capitalSpriteArray[0];
        }
        else village.GetComponent<SpriteRenderer>().sprite = capitalSpriteArray[1];
    }
}
