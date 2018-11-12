using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour {
	public static bool hasUnit = false;
	public static Vector3 mousePos;
	public static Person person;
    public static Ship ship;
    public static Castle castle;
    public static Harbor harbor;
    public static GameObject village;
    public static List<Hexagon> viableHexagons;
	public GameObject hexObject;
    bool pickedUpIsValid;
    private GameObject parent;
    private bool clickedHexagonHasCountry;
    public static bool pickUpBuffer;
    float pickUpBufferCounter = .5f;

	private void Start()
	{
        viableHexagons = new List<Hexagon>();
	}
	void Update() {
		mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1);
		mousePos = Camera.main.ScreenToWorldPoint (mousePos);
		this.transform.position = mousePos;
        if (pickUpBuffer){
            pickUpBufferCounter -= Time.deltaTime;
            if (pickUpBufferCounter <= 0){
                pickUpBuffer = false;
                pickUpBufferCounter = .5f;
            }
        }
	}
	void OnTriggerStay2D(Collider2D col){
        try
        {
            parent = col.gameObject.transform.parent.gameObject;
            clickedHexagonHasCountry = true;
        }
        catch (Exception) { clickedHexagonHasCountry = false; }
        if (col.gameObject.tag.Equals("person") && Input.GetMouseButtonDown(0) && !hasUnit)
        {
            person = col.GetComponent<Person>();
            pickedUpIsValid = PickedUpIsValid("person");
            if (pickedUpIsValid) PickUpUnit("person");
            if (UIManager.showingShipInfo) UIManager.HideShipInfo();
        }
        else if (village == null && col.gameObject.tag.Equals("capital") && Input.GetMouseButtonDown(0) && UIManager.turnNumber == col.gameObject.transform.parent.GetComponent<Hexagon>().nationNum)
        {
            if (UIManager.showingShipInfo)
            {
                UIManager.HideShipInfo();
            }
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UIManager.SetCurrentCountry(parent.transform.parent.gameObject);
            }
        }
        else if (col.gameObject.tag.Equals("ship") && Input.GetMouseButtonDown(0))
        {
            if (col.gameObject.GetComponent<Ship>().moored)
            { //selecting an invasion country because the ship is the temporary capital
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    UIManager.SetCurrentCountry(col.transform.parent.gameObject);
                }
                return;
            }
            if (!hasUnit)
            { //normal pick up ship
                ship = col.GetComponent<Ship>();
                pickedUpIsValid = PickedUpIsValid("ship");
                if (pickedUpIsValid)
                {
                    PickUpUnit("ship");
                    UIManager.ShowBoatInfo(ship);
                }
            }
        }
        else if (col.gameObject.tag.Equals("ship") && Input.GetMouseButtonDown(1)){
            ship = col.GetComponent<Ship>();
            UIManager.ShowBoatInfo(ship);
        }
	}
	void PickedUpBuffer(){
        hasUnit = true;
        pickUpBuffer = true;
	}
    public void PickUpUnit(string s){
        if (s == "person")
        {
            if (person.redDot != null){ person.redDot.layer = 9; }
            person.GetComponent<CircleCollider2D>().radius = .0036f;
            this.Invoke("PickedUpBuffer", .1f);
            person.hover = true;

            if (!person.usingFirstTurn)
            { //here forward is for showing the viable places to place a person
                List<Hexagon> addingViableHexes = new List<Hexagon>();
                viableHexagons.Clear();
                Hexagon possibleHex;
                viableHexagons.Add(person.hexagon);
                person.hexagon.distanceFromStartingPoint = 0;
                addingViableHexes.Add(person.hexagon);
                print(person.hexagon);
                for (int i = UIManager.personMovementSpeed; i > 0; i--)
                {
                    for (int j = addingViableHexes.Count - 1; j >= 0; j--)
                    {
                        Hexagon h = addingViableHexes[j];
                        foreach (GameObject g in h.adj)
                        {
                            possibleHex = g.GetComponent<Hexagon>();
                            if ((viableHexagons.Contains(possibleHex) || addingViableHexes.Contains(possibleHex)) == false){
                                
                                bool isAdjacentToParentCountry = false;
                                bool hasAViableUpgradeGuard = true;
                                bool hasFriendlyBuilding = false;
                                bool personCanCaptureEnemy = true;
                                bool isRoundOne = false;

                                if (possibleHex.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                else
                                {
                                    Hexagon checkForAdjacency;
                                    foreach (GameObject l in possibleHex.adj)
                                    {
                                        checkForAdjacency = l.GetComponent<Hexagon>();
                                        if (checkForAdjacency.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                        if (checkForAdjacency.hasShip)
                                        {
                                            if (checkForAdjacency.ship.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                        }
                                    }
                                }

                                if (possibleHex.hasGuard)
                                {
                                    if (person.tier + possibleHex.guard.tier > 4) { hasAViableUpgradeGuard = false; }
                                }

                                if (possibleHex.transform.parent == person.transform.parent)
                                {
                                    if (possibleHex.hasCastle || possibleHex.hasHarbor || possibleHex.hasCapital) { hasFriendlyBuilding = true; }
                                }

                                if (possibleHex.transform.parent != person.transform.parent)
                                {
                                    if ((possibleHex.guardedBy < person.tier || possibleHex.guardedBy == 0) == false && possibleHex.isLand) { personCanCaptureEnemy = false; }
                                }

                                if (possibleHex.transform.parent != person.transform.parent && UIManager.roundNumber == 1) { isRoundOne = true; }
                                if (possibleHex.isLand)
                                {
                                    print(possibleHex);
                                    print(isAdjacentToParentCountry + " " + personCanCaptureEnemy + " " + !isRoundOne);
                                    if (isAdjacentToParentCountry && personCanCaptureEnemy && !isRoundOne)
                                    {
                                        if (possibleHex.transform.parent == person.transform.parent)
                                        {
                                            addingViableHexes.Add(possibleHex);
                                        }
                                        if (!hasFriendlyBuilding && hasAViableUpgradeGuard)
                                        { //so that you can reach parts of your country between you and a building
                                            viableHexagons.Add(possibleHex);
                                        }
                                        if (possibleHex != person.hexagon && possibleHex.distanceFromStartingPoint == -1)
                                        {
                                            possibleHex.distanceFromStartingPoint = UIManager.personMovementSpeed + 1 - i;
                                        }
                                    }
                                }
                                else if (possibleHex.hasShip)
                                {
                                    if (possibleHex.ship.isHarbored && possibleHex.ship.nationNum == person.nationNum && possibleHex.ship.sailors.Count < 10) { 
                                        viableHexagons.Add(possibleHex);
                                        if (possibleHex != person.hexagon && possibleHex.distanceFromStartingPoint == -1)
                                        {
                                            possibleHex.distanceFromStartingPoint = UIManager.personMovementSpeed + 1 - i;
                                        }
                                    }
                                } 
                            }
                        }
                    }
                }
                foreach (Hexagon h in viableHexagons)
                {
                    h.ToggleMask();
                }
            }
            else {
                List<Hexagon> addingViableHexes = new List<Hexagon>();
                Hexagon possibleHex;
                if (UIManager.selectedCountryScript.isInvasionCountry){
                    UIManager.selectedCountryScript.hexagons.Add(UIManager.selectedCountryScript.capital);
                }
                foreach (Hexagon h in UIManager.selectedCountryScript.hexagons){
                    if (h.hasCapital || h.hasCastle || h.hasHarbor || h.hasShip) {
                        addingViableHexes.Add(h);
                        h.distanceFromStartingPoint = 0;
                    }
                }
                for (int i = UIManager.personMovementSpeed; i > 0; i--)
                {
                    for (int j = addingViableHexes.Count - 1; j >= 0; j--)
                    {
                        Hexagon h = addingViableHexes[j];
                        foreach (GameObject g in h.adj)
                        {
                            possibleHex = g.GetComponent<Hexagon>();
                            if ((viableHexagons.Contains(possibleHex) || addingViableHexes.Contains(possibleHex)) == false)
                            {

                                bool isAdjacentToParentCountry = false;
                                bool hasAViableUpgradeGuard = true;
                                bool hasFriendlyBuilding = false;
                                bool personCanCaptureEnemy = true;
                                bool isRoundOne = false;

                                if (possibleHex.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                else
                                {
                                    Hexagon checkForAdjacency;
                                    foreach (GameObject l in possibleHex.adj)
                                    {
                                        checkForAdjacency = l.GetComponent<Hexagon>();
                                        if (checkForAdjacency.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                        if (checkForAdjacency.hasShip)
                                        {
                                            if (checkForAdjacency.ship.transform.parent == person.transform.parent) { isAdjacentToParentCountry = true; }
                                        }
                                    }
                                }

                                if (possibleHex.hasGuard)
                                {
                                    if (person.tier + possibleHex.guard.tier > 4) { hasAViableUpgradeGuard = false; }
                                }

                                if (possibleHex.transform.parent == person.transform.parent)
                                {
                                    if (possibleHex.hasCastle || possibleHex.hasHarbor || possibleHex.hasCapital) { hasFriendlyBuilding = true; }
                                }

                                if (possibleHex.transform.parent != person.transform.parent)
                                {
                                    if ((possibleHex.guardedBy < person.tier || possibleHex.guardedBy == 0) == false && possibleHex.isLand) { personCanCaptureEnemy = false; }
                                }

                                if (possibleHex.transform.parent != person.transform.parent && UIManager.roundNumber == 1) { isRoundOne = true; }
                                if (possibleHex.isLand)
                                {
                                    //print(possibleHex);
                                    //print(isAdjacentToParentCountry + " " + personCanCaptureEnemy + " " + hasAViableUpgradeGuard + " " + !isRoundOne);
                                    if (isAdjacentToParentCountry && personCanCaptureEnemy && hasAViableUpgradeGuard && !isRoundOne)
                                    {
                                        if (possibleHex.transform.parent == person.transform.parent)
                                        {
                                            addingViableHexes.Add(possibleHex);

                                        }
                                        if (!hasFriendlyBuilding)
                                        { //so that you can reach parts of your country between you and a building
                                            viableHexagons.Add(possibleHex);
                                            if (possibleHex != person.hexagon && possibleHex.distanceFromStartingPoint == -1)
                                            {
                                                possibleHex.distanceFromStartingPoint = UIManager.personMovementSpeed + 1 - i;
                                            }
                                        }
                                    }
                                }
                                else if (possibleHex.hasShip)
                                {
                                    if (possibleHex.ship.isHarbored && possibleHex.ship.nationNum == person.nationNum && possibleHex.ship.sailors.Count < 10) { viableHexagons.Add(possibleHex); }
                                }
                            }
                        }
                    }
                }
                       
                if (UIManager.selectedCountryScript.isInvasionCountry)
                {
                    UIManager.selectedCountryScript.hexagons.Remove(UIManager.selectedCountryScript.capital);
                }
                foreach (Hexagon h in viableHexagons){
                    h.ToggleMask();
                }
            }
        }
        else if (s == "ship")
        { 
            if (ship.redDot != null) { ship.redDot.layer = 9; }
            ship.GetComponent<CircleCollider2D>().radius = .0036f;
            if (ship.oldHexagon != null){
                ship.oldHexagon.ToggleShipColor(ship.nationNum);
            }
            this.Invoke("PickedUpBuffer", .1f);
            ship.hover = true;
            UIManager.HideInvasionButton();
            ship.invasionPossibility = false;
            if (!ship.isFirstTurn)
            { //here forward is for showing the viable places to place a ship
                List<Hexagon> addingViableHexes = new List<Hexagon>();
                Hexagon possibleHex;
                viableHexagons.Add(ship.hexagon);
                ship.hexagon.distanceFromStartingPoint = 0;
                addingViableHexes.Add(ship.hexagon);
                for (int i = UIManager.shipMovementSpeed; i > 0; i--)
                {
                    for (int j = addingViableHexes.Count - 1; j >= 0; j--)
                    {
                        Hexagon h = addingViableHexes[j];
                        foreach (GameObject g in h.adj)
                        {
                            if (g != null){
                                possibleHex = g.GetComponent<Hexagon>();
                                if (!possibleHex.isLand && !addingViableHexes.Contains(possibleHex))
                                {
                                    addingViableHexes.Add(possibleHex);
                                    viableHexagons.Add(possibleHex);
                                    if (possibleHex != ship.hexagon && possibleHex.distanceFromStartingPoint == -1)
                                    {
                                        possibleHex.distanceFromStartingPoint = UIManager.shipMovementSpeed + 1 - i;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (Hexagon h in viableHexagons)
                {
                    h.ToggleMask();
                }
            }
            else{
                foreach(Hexagon h in ship.transform.parent.GetComponent<Country>().hexagons){
                    if(h.hasHarbor){
                        foreach(GameObject g in h.adj){
                            Hexagon waterHex = g.GetComponent<Hexagon>();
                            if (!waterHex.isLand && !viableHexagons.Contains(waterHex)){
                                viableHexagons.Add(waterHex);
                            }
                        }
                    }
                }
                foreach (Hexagon h in viableHexagons)
                {
                    h.ToggleMask();
                }
            }
        }
        else if (s == "castle"){
            castle.GetComponent<CircleCollider2D>().radius = .0036f;
            this.Invoke("PickedUpBuffer", .1f);
            castle.hover = true;
            viableHexagons.Clear();
            //show viable spots to place
            foreach(Hexagon h in UIManager.selectedCountryScript.hexagons){
                if (!h.hasCastle && !h.hasHarbor && !h.hasCapital && !h.hasGuard && !h.hasGrave){
                    viableHexagons.Add(h);
                }
            }
            foreach (Hexagon h in viableHexagons)
            {
                h.ToggleMask();
            }
        }
        else if (s == "harbor"){
            harbor.GetComponent<CircleCollider2D>().radius = .0036f;
            this.Invoke("PickedUpBuffer", .1f);
            harbor.hover = true;  
            viableHexagons.Clear();
            foreach (Hexagon h in UIManager.selectedCountryScript.hexagons)
            {
                if (!h.hasCastle && !h.hasHarbor && !h.hasCapital && !h.hasGuard && !h.hasGrave){
                    foreach (GameObject g in h.adj)
                    {
                        Hexagon hexa = g.GetComponent<Hexagon>();
                        if (!hexa.isLand)
                        {
                            viableHexagons.Add(h);
                            break;
                        }
                    }
                }
            }
            foreach (Hexagon h in viableHexagons)
            {
                h.ToggleMask();
            }
        }
        else if (s == "village"){
            this.Invoke("PickedUpBuffer", .1f);
            viableHexagons.Clear();
            foreach(Hexagon h in UIManager.selectedCountryScript.hexagons){
                if(!h.hasCastle && !h.hasHarbor && !h.hasCapital && !h.hasGuard && !h.hasGrave){
                    viableHexagons.Add(h);
                }
            }
            foreach (Hexagon h in viableHexagons)
            {
                h.ToggleMask();
            }
        }
    }
    public bool PickedUpIsValid(string s){
        if(s == "person"){
            if (UIManager.selectedCountry == person.transform.parent.gameObject && person.hasUsedMove == false && person.hover == false && hasUnit == false)
            {
                return true;
            }
            return false;   
        }
        if(s == "ship"){
            if (UIManager.turnNumber == ship.nationNum && !ship.hasUsedMove && !ship.hover && !hasUnit){
                return true;
            }
            return false;
        }
        return false;
    }
    public static void ResetViableHexagons()
    {
        foreach (Hexagon h in viableHexagons)
        {
            h.ToggleMask();
            h.distanceFromStartingPoint = -1;
        }
        viableHexagons.Clear();
    }
}
