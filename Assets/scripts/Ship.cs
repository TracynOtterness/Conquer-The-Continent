using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public Sprite[] sprites;
    private Sprite sprite;

    public List<Person> sailors;

    List<Hexagon> pathHexes;
    int currentStep;
    Vector2 hexDistance;
    bool doMarch;

    float time = 1f;
    float movementTimer = 0;
    float movementSpeed = .5f;
    Vector3 pos;
    public bool hover;
    public bool displayingInfo;
    public bool hasUsedMove = false;
    public bool isFirstTurn = true;
    public bool invasionPossibility = false;
    public bool isHarbored = false;
    public bool moored;
    public int nationNum;
    public int food;
    public int peasantCount;
    public static int foodCap = 80;
    public static int fishValue = 4;
    public static int sailorFoodConsumption = 1;
    public GameObject hex;
    public Hexagon hexagon;
    public GameObject harbor;
    public Hexagon oldHexagon;
    public GameObject redDot = null;
    public GameObject invasionCountry = null;

	private void Start () {
        sailors = new List<Person>(10);
        pathHexes = new List<Hexagon>();
        sprites = Resources.LoadAll<Sprite>("ship");
        nationNum = UIManager.turnNumber;
        name = "Unit " + UIManager.selectedCountry + " , " + UIManager.selectedCountry.GetComponent<Country>().unitCount;
        UIManager.selectedCountryScript.unitCount++;
        transform.SetParent(UIManager.selectedCountry.transform);
	}
	
	// Update is called once per frame
	void Update () {
        if(!hasUsedMove && !hover){
            time -= Time.deltaTime;
            if (time <= 0){
                if (sprites[0] == sprite){
                    sprite = sprites[1];
                    GetComponent<SpriteRenderer>().sprite = sprites[1];
                }
                else{
                    sprite = sprites[0];
                    GetComponent<SpriteRenderer>().sprite = sprites[0];
                }
                time = 1f;
            }
        }
        if(hover && UIManager.hoverMode){
            this.transform.position = Mouse.mousePos;
        }

        if (doMarch)
        {
            movementTimer += Time.deltaTime;
            if (movementTimer >= movementSpeed)
            {
                this.transform.position = pathHexes[currentStep + 1].transform.position;
                movementTimer = 0f;
                currentStep++;
                if (currentStep + 1 > pathHexes.Count - 1)
                {
                    doMarch = false;
                    Place();
                }
                else
                {
                    hexDistance = new Vector2((pathHexes[currentStep + 1].transform.position.x - pathHexes[currentStep].transform.position.x), (pathHexes[currentStep + 1].transform.position.y - pathHexes[currentStep].transform.position.y));
                }
            }
            else
            {
                this.transform.position = new Vector3((pathHexes[currentStep].transform.position.x + (1/movementSpeed) * movementTimer * hexDistance.x), (pathHexes[currentStep].transform.position.y + (1/movementSpeed) * movementTimer * hexDistance.y));//transform = hex that person is coming from + percentage of time to .5 seconds * hexdistance; 
            }
        }
	}
    private void OnTriggerStay2D(Collider2D col) {
        if (hover && col.gameObject.tag.Equals("hex")){
                hex = col.gameObject;
                hexagon = hex.GetComponent<Hexagon>();
        }
    }
    public void MoveShip(GameObject g){
        //getting gameobject and hexagon it is being placed on
        hex = g;
        hexagon = g.GetComponent<Hexagon>();
        //adjusting hitbox for new situation
        this.GetComponent<CircleCollider2D>().radius = .175f;
        if (UIManager.hoverMode)
        {
            //changing transform position for ship and attached redDot
            pos = hex.transform.position;
            this.transform.position = pos;
            pos.z = -1;
            Place();
        }
        else if (isFirstTurn)
        {
            //changing transform position for ship and attached redDot
            print("doot");
            pos = hex.transform.position;
            this.transform.position = pos;
            pos.z = -1;
            Mouse.ResetViableHexagons();
            isFirstTurn = false;
            Place();
        }
        else
        {
            GetPath();
            if (pathHexes.Count == 1)
            {//it is being placed on the same tile
                Place();
            }
            else
            {
                pathHexes.Reverse();
                foreach (Hexagon h in pathHexes)
                {
                    print(h);
                }
                hexDistance = new Vector2((pathHexes[1].transform.position.x - pathHexes[0].transform.position.x), (pathHexes[1].transform.position.y - pathHexes[0].transform.position.y));
                doMarch = true;
            }
        }
        Mouse.ResetViableHexagons();
    }
    public void Place(){
        hexagon.ToggleShipColor(nationNum);
        hexagon.hasShip = true;
        hexagon.ship = this;

        if (redDot != null){
            redDot.transform.position = pos;
            if (UIManager.toggled == true) redDot.layer = 0;  
        }
        if (oldHexagon != null && oldHexagon != hexagon){
            oldHexagon.hasShip = false;
            oldHexagon.ship = null;
        }
        oldHexagon = hexagon;
        isHarbored = false;
        foreach (GameObject adjHex in hexagon.adj){ //reset isHarbored to true if it is, also determine if an invasion possibility exists.
            if (adjHex != null){
                Hexagon h = adjHex.GetComponent<Hexagon>();
                if (h.nationNum == nationNum && h.hasHarbor)
                {
                    harbor = h.harbor;
                    isHarbored = true;
                }
                else if (h.isLand && !isHarbored && !moored && (h.nationNum != nationNum || !h.isCountry))
                {
                    invasionPossibility = true;
                } 
            }
        }
        if(!isHarbored){
            UIManager.ShowBoatInfo(this);
        }
    }
    public void NewRound() {
        isHarbored = false;
        foreach(GameObject g in hexagon.adj){
            if (g != null){
                Hexagon adjHexagon = g.GetComponent<Hexagon>();
                if (adjHexagon.hasHarbor && adjHexagon.nationNum == nationNum)
                {
                    harbor = adjHexagon.harbor;
                    isHarbored = true;
                }
            }
        }
        if (!moored){
            foreach (Person p in sailors)
            {
                if (p.tier == 1)
                {
                    food += fishValue;
                }
                food -= sailorFoodConsumption;
            }
            if (food > foodCap)
            {
                food = foodCap;
            }
            //if (food < 0) ShipWreck();  
        }
    }

    void GetPath()
    {
        currentStep = 0;
        pathHexes.Clear();
        print(hexagon);
        pathHexes.Add(hexagon);
        Hexagon lastHex = hexagon;
        for (int i = hexagon.distanceFromStartingPoint; i > 0; i--)
        {
            foreach (GameObject g in lastHex.adj)
            {
                if (g != null){
                    Hexagon h = g.GetComponent<Hexagon>();
                    if (h.distanceFromStartingPoint + 1 == i)
                    {
                        print(lastHex + " is adjacent to " + h + ", adding " + h + " to pathhexes");
                        pathHexes.Add(h);
                        lastHex = h;
                        break;
                    }
                }
            }
        }
    }
}
