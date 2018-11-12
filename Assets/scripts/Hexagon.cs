using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hexagon : MonoBehaviour {
	public int HexPosX;
	public int HexPosY;
    public int nationNum;
    private float b = .279f;
	private Sprite[] spriteArray;
    public GameObject[] adj = new GameObject[6];
    public int guardedBy = 0;
    public bool isLand = false;
    public bool isCountry = false;
    public bool hasGuard;
    public bool hasCapital;
    public bool hasGrave;
    public bool hasCastle;
    public bool hasHarbor;
    public bool hasShip;
    bool maskToggled;
    public GameObject village;
    public GameObject shipCapital;
    public GameObject castle;
    public GameObject harbor;
    public GameObject grave;
    public Sprite[]capitalSpriteArray = new Sprite[2];
    public Person guard;
    public Ship ship;
    public int distanceFromStartingPoint;

	void Awake () {
		spriteArray = Resources.LoadAll<Sprite>("Hexes");
		HexPosX = Instantiator.HexPosX;
		HexPosY = Instantiator.HexPosY;
        distanceFromStartingPoint = -1;
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
        nationNum = Random.Range(1 , UIManager.playerCount + 1);
		this.GetComponent<SpriteRenderer> ().sprite = spriteArray [nationNum];
		Instantiator.totalHexes++;
}
    public void ChangeSprite(int a){
        this.GetComponent<SpriteRenderer>().sprite = spriteArray[a];
    }
    public void AddCapital(){ 
            hasCapital = true;
            foreach(Transform t in this.transform){
            if(t.tag == "capital"){
                village = t.gameObject;
            }
            }
            print(this + " is now a capital. Village is " + village);
            guardedBy = 1; 
    }
    public void RemoveCapital(){
        hasCapital = false;
        print(village);
        Destroy(village);
        print("you destroyed the capital!");
    }
    public void EditFlag(){
        if (village == null) return;
        if (transform.parent.GetComponent<Country>().food >= 10){
            village.GetComponent<SpriteRenderer>().sprite = capitalSpriteArray[0];
        }
        else village.GetComponent<SpriteRenderer>().sprite = capitalSpriteArray[1];
    }
    public void ToggleMask(){
        if (maskToggled){
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            maskToggled = false;
        }
        else{
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            maskToggled = true;
        }
    }
    public void ToggleShipColor(int shipNationNum){
        print("ToggleShipColor");
        if (transform.GetChild(1).GetComponent<SpriteRenderer>().enabled == true){
            transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        }
        else{
            transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            switch(shipNationNum){
                case 1: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(6, 240, 255, 255); break;
                case 2: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 255, 255); break;
                case 3: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(0, 255, 8, 255); break;
                case 4: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 0, 255); break;
                case 5: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255); break;
                case 6: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(21, 134, 24, 255); break;
                case 7: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 174, 0, 255); break;
                case 8: transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(121, 22, 177, 255); break;
            }
        }
    }
}
